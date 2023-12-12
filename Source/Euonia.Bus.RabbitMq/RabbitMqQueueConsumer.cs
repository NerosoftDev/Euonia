using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Nerosoft.Euonia.Bus.RabbitMq;

/// <summary>
/// The RabbitMQ implementation of <see cref="IQueueConsumer"/>.
/// </summary>
public class RabbitMqQueueConsumer : RabbitMqQueueRecipient, IQueueConsumer
{
	private readonly IIdentityProvider _identity;

	/// <summary>
	/// Initializes a new instance of the <see cref="RabbitMqQueueConsumer"/> class.
	/// </summary>
	/// <param name="connection"></param>
	/// <param name="handler"></param>
	/// <param name="options"></param>
	public RabbitMqQueueConsumer(IPersistentConnection connection, IHandlerContext handler, IOptions<RabbitMqMessageBusOptions> options)
		: base(connection, handler, options)
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="RabbitMqQueueConsumer"/> class.
	/// </summary>
	/// <param name="connection"></param>
	/// <param name="handler"></param>
	/// <param name="options"></param>
	/// <param name="identity"></param>
	public RabbitMqQueueConsumer(IPersistentConnection connection, IHandlerContext handler, IOptions<RabbitMqMessageBusOptions> options, IIdentityProvider identity)
		: this(connection, handler, options)
	{
		_identity = identity;
	}

	/// <inheritdoc />
	public string Name => nameof(RabbitMqQueueConsumer);

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
		if (!Connection.IsConnected)
		{
			Connection.TryConnect();
		}

		Channel = Connection.CreateChannel();

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

		var context = new MessageContext(message, _identity.GetIdentity);

		OnMessageReceived(new MessageReceivedEventArgs(message.Data, context));

		var taskCompletion = new TaskCompletionSource<object>();
		context.Responded += (_, a) =>
		{
			taskCompletion.TrySetResult(a.Result);
		};
		context.Failed += (_, exception) =>
		{
			taskCompletion.TrySetException(exception);
		};
		context.Completed += (_, _) =>
		{
			taskCompletion.TryCompleteFromCompletedTask(Task.FromResult(default(object)));
		};

		RabbitMqReply<object> reply;

		await Handler.HandleAsync(message.Channel, message.Data, context);

		try
		{
			var result = await taskCompletion.Task;
			reply = RabbitMqReply<object>.Success(result);
		}
		catch (Exception exception)
		{
			reply = RabbitMqReply<object>.Failure(exception);
		}

		if (!string.IsNullOrEmpty(props.CorrelationId) || !string.IsNullOrWhiteSpace(props.ReplyTo))
		{
			var replyProps = Channel.CreateBasicProperties();
			replyProps.Headers ??= new Dictionary<string, object>();
			replyProps.CorrelationId = props.CorrelationId;

			var response = SerializeMessage(reply);
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