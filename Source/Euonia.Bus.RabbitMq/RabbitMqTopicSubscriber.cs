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
	/// Initializes a new instance of the <see cref="RabbitMqTopicSubscriber"/> class.
	/// </summary>
	/// <param name="connection"></param>
	/// <param name="handler"></param>
	/// <param name="options"></param>
	public RabbitMqTopicSubscriber(IPersistentConnection connection, IHandlerContext handler, IOptions<RabbitMqMessageBusOptions> options)
		: base(connection, handler, options)
	{
	}

	/// <inheritdoc />
	public string Name => nameof(RabbitMqTopicSubscriber);

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
		if (!Connection.IsConnected)
		{
			Connection.TryConnect();
		}

		Channel = Connection.CreateChannel();

		string queueName;
		if (string.IsNullOrWhiteSpace(Options.TopicName))
		{
			Channel.ExchangeDeclare(channel, Options.ExchangeType);
			queueName = Channel.QueueDeclare().QueueName;
		}
		else
		{
			Channel.QueueDeclare(Options.TopicName, true, false, false, null);
			queueName = Options.TopicName;
		}

		Consumer = new EventingBasicConsumer(Channel);
		Consumer.Received += HandleMessageReceived;

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

	/// <inheritdoc />
	protected override void Dispose(bool disposing)
	{
		if (!disposing)
		{
			return;
		}

		Consumer.Received -= HandleMessageReceived;
		Channel?.Dispose();
	}
}