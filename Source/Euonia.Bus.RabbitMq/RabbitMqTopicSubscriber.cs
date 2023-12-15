using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Nerosoft.Euonia.Bus.RabbitMq;

/// <summary>
/// 
/// </summary>
public class RabbitMqTopicSubscriber : RabbitMqQueueRecipient, ITopicSubscriber
{
	private readonly IIdentityProvider _identity;
	private readonly IHandlerContext _handler;
	private readonly ILogger<RabbitMqTopicSubscriber> _logger;
	/// <summary>
	/// Initializes a new instance of the <see cref="RabbitMqTopicSubscriber"/> class.
	/// </summary>
	/// <param name="connection"></param>
	/// <param name="handler"></param>
	/// <param name="options"></param>
	/// <param name="logger"></param>
	public RabbitMqTopicSubscriber(IPersistentConnection connection, IHandlerContext handler, IOptions<RabbitMqMessageBusOptions> options, ILoggerFactory logger)
		: base(connection, options)
	{
		_handler = handler;
		_logger = logger.CreateLogger<RabbitMqTopicSubscriber>();
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="RabbitMqTopicSubscriber"/> class.
	/// </summary>
	/// <param name="connection"></param>
	/// <param name="handler"></param>
	/// <param name="options"></param>
	/// <param name="logger"></param>
	/// <param name="identity"></param>
	public RabbitMqTopicSubscriber(IPersistentConnection connection, IHandlerContext handler, IOptions<RabbitMqMessageBusOptions> options, ILoggerFactory logger, IIdentityProvider identity)
		: this(connection, handler, options, logger)
	{
		_identity = identity;
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

		var context = new MessageContext(message, authorization => _identity?.GetIdentity(authorization));

		OnMessageReceived(new MessageReceivedEventArgs(message.Data, context));

		await HandleAsync(message.Channel, message.Data, context);

		if (!Options.AutoAck)
		{
			Channel.BasicAck(args.DeliveryTag, false);
		}

		OnMessageAcknowledged(new MessageAcknowledgedEventArgs(message.Data, context));
	}

	/// <inheritdoc />
	protected override async Task HandleAsync(string channel, object message, MessageContext context, CancellationToken cancellationToken = default)
	{
		try
		{
			await _handler.HandleAsync(channel, message, context, cancellationToken);
		}
		catch (Exception exception)
		{
			_logger.LogError(exception, "Message '{Id}' Handle Error: {Message}", context.MessageId, exception.Message);
		}
		finally
		{
		}
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