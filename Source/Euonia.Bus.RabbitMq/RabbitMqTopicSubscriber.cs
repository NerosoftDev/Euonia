using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Nerosoft.Euonia.Bus.RabbitMq;

/// <summary>
/// 
/// </summary>
public class RabbitMqTopicSubscriber : RabbitMqQueueRecipient, ITopicSubscriber
{
	/// <summary>
	/// Initializes a new instance of the &lt;see cref="RabbitMqTopicSubscriber"/&gt; class.
	/// </summary>
	/// <param name="connection"></param>
	/// <param name="handler"></param>
	/// <param name="options"></param>
	public RabbitMqTopicSubscriber(IConnection connection, IHandlerContext handler, IOptions<RabbitMqMessageBusOptions> options)
		: base(connection, handler, options)
	{
	}

	/// <inheritdoc />
	public string Name => nameof(RabbitMqTopicSubscriber);

	internal override void Start(string channel)
	{
		string queueName;
		if (string.IsNullOrWhiteSpace(Options.EventQueueName))
		{
			Channel.ExchangeDeclare(channel, Options.ExchangeType);
			queueName = Channel.QueueDeclare().QueueName;
		}
		else
		{
			Channel.QueueDeclare(Options.EventQueueName, true, false, false, null);
			queueName = Options.EventQueueName;
		}
		
		Channel.QueueBind(queueName, channel, Options.RoutingKey ?? "*");
		Channel.BasicConsume(string.Empty, Options.AutoAck, Consumer);
	}

	/// <inheritdoc />
	protected override async void HandleMessageReceived(object sender, BasicDeliverEventArgs args)
	{
		var type = MessageTypeCache.GetMessageType(args.BasicProperties.Type);

		var message = DeserializeMessage(args.Body.ToArray(), type);

		var context = new MessageContext();

		OnMessageReceived(new MessageReceivedEventArgs(message.Data, context));

		await Handler.HandleAsync(message.Channel, message.Data, context);

		if (!Options.AutoAck)
		{
			Channel.BasicAck(args.DeliveryTag, false);
		}

		OnMessageAcknowledged(new MessageAcknowledgedEventArgs(message.Data, context));
	}
}