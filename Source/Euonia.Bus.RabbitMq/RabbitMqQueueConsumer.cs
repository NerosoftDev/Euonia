using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Nerosoft.Euonia.Bus.RabbitMq;

/// <summary>
/// 
/// </summary>
public class RabbitMqQueueConsumer : RabbitMqQueueRecipient, IQueueConsumer
{
	/// <summary>
	/// Initializes a new instance of the &lt;see cref="RabbitMqQueueConsumer"/&gt; class.
	/// </summary>
	/// <param name="connection"></param>
	/// <param name="handler"></param>
	/// <param name="options"></param>
	public RabbitMqQueueConsumer(IConnection connection, IHandlerContext handler, IOptions<RabbitMqMessageBusOptions> options)
		: base(connection, handler, options)
	{
	}

	/// <inheritdoc />
	public string Name => nameof(RabbitMqQueueConsumer);

	internal override void Start(string channel)
	{
		var queueName = $"{Options.QueueName}${channel}$";
		Channel.QueueDeclare(queueName, true, false, false, null);
		Channel.BasicQos(0, 1, false);
		Channel.BasicConsume(channel, Options.AutoAck, Consumer);
	}

	/// <inheritdoc />
	protected override async void HandleMessageReceived(object sender, BasicDeliverEventArgs args)
	{
		var type = MessageTypeCache.GetMessageType(args.BasicProperties.Type);

		var message = DeserializeMessage(args.Body.ToArray(), type);

		var props = args.BasicProperties;
		var replyNeeded = !string.IsNullOrEmpty(props.CorrelationId);

		var context = new MessageContext();

		OnMessageReceived(new MessageReceivedEventArgs(message.Data, context));

		var taskCompletion = new TaskCompletionSource<object>();
		context.OnResponse += (_, a) =>
		{
			taskCompletion.TrySetResult(a.Result);
		};
		context.Completed += (_, _) =>
		{
			if (!Options.AutoAck)
			{
				Channel.BasicAck(args.DeliveryTag, false);
			}
		};

		await Handler.HandleAsync(message.Channel, message.Data, context);

		if (replyNeeded)
		{
			var result = await taskCompletion.Task;
			var replyProps = Channel.CreateBasicProperties();
			replyProps.Headers ??= new Dictionary<string, object>();
			replyProps.Headers.Add(Constants.MessageHeaders.MessageType, result.GetType().GetFullNameWithAssemblyName());
			replyProps.CorrelationId = props.CorrelationId;

			var response = SerializeMessage(result);
			Channel.BasicPublish(string.Empty, props.ReplyTo, replyProps, response);
			Channel.BasicAck(args.DeliveryTag, false);
		}
		else
		{
			taskCompletion.SetCanceled();
		}

		OnMessageAcknowledged(new MessageAcknowledgedEventArgs(message.Data, context));
	}
}