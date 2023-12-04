using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Nerosoft.Euonia.Bus.RabbitMq;

/// <summary>
/// The RabbitMQ implementation of <see cref="IQueueConsumer"/>.
/// </summary>
public class RabbitMqQueueConsumer : RabbitMqQueueRecipient, IQueueConsumer
{
	/// <summary>
	/// Initializes a new instance of the <see cref="RabbitMqQueueConsumer"/> class.
	/// </summary>
	/// <param name="factory"></param>
	/// <param name="handler"></param>
	/// <param name="options"></param>
	public RabbitMqQueueConsumer(ConnectionFactory factory, IHandlerContext handler, IOptions<RabbitMqMessageBusOptions> options)
		: base(factory, handler, options)
	{
	}

	/// <inheritdoc />
	public string Name => nameof(RabbitMqQueueConsumer);

	private IConnection Connection { get; set; }

	/// <summary>
	/// Gets the RabbitMQ message channel.
	/// </summary>
	private IModel Channel { get; set; }

	/// <summary>
	/// Gets the RabbitMQ consumer instance.
	/// </summary>
	private EventingBasicConsumer Consumer { get; set; }

	internal override void Start(string channel)
	{
		var queueName = $"{Options.QueueName}${channel}$";

		Connection = ConnectionFactory.CreateConnection();

		Channel = Connection.CreateModel();

		Channel.QueueDeclare(queueName, true, false, false, null);
		Channel.BasicQos(0, 1, false);

		Consumer = new EventingBasicConsumer(Channel);
		Consumer.Received += HandleMessageReceived;

		Channel.BasicConsume(queueName, Options.AutoAck, Consumer);
	}

	/// <inheritdoc />
	protected override async void HandleMessageReceived(object sender, BasicDeliverEventArgs args)
	{
		var type = MessageTypeCache.GetMessageType(args.BasicProperties.Type);

		var message = DeserializeMessage(args.Body.ToArray(), type);

		var props = args.BasicProperties;

		var context = new MessageContext();

		OnMessageReceived(new MessageReceivedEventArgs(message.Data, context));

		var taskCompletion = new TaskCompletionSource<object>();
		context.OnResponse += (_, a) =>
		{
			taskCompletion.TrySetResult(a.Result);
		};
		context.Completed += (_, _) =>
		{
			taskCompletion.TryCompleteFromCompletedTask(Task.FromResult(default(object)));
		};

		await Handler.HandleAsync(message.Channel, message.Data, context);

		var result = await taskCompletion.Task;

		if (!string.IsNullOrEmpty(props.CorrelationId) || !string.IsNullOrWhiteSpace(props.ReplyTo))
		{
			var replyProps = Channel.CreateBasicProperties();
			replyProps.Headers ??= new Dictionary<string, object>();
			replyProps.Headers.Add(Constants.MessageHeaders.MessageType, result.GetType().GetFullNameWithAssemblyName());
			replyProps.CorrelationId = props.CorrelationId;

			var response = SerializeMessage(result);
			Channel.BasicPublish(string.Empty, props.ReplyTo, replyProps, response);
		}

		Channel.BasicAck(args.DeliveryTag, false);

		OnMessageAcknowledged(new MessageAcknowledgedEventArgs(message.Data, context));
	}

	/// <inheritdoc />
	protected override void Dispose(bool disposing)
	{
		if (!disposing)
		{
			return;
		}

		Consumer.Received -= HandleMessageReceived;
		Channel?.Dispose();
		Connection?.Dispose();
	}
}