using Microsoft.Extensions.Logging;
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
	private readonly IHandlerContext _handler;
	private readonly ILogger<RabbitMqQueueConsumer> _logger;

	/// <summary>
	/// Initializes a new instance of the <see cref="RabbitMqQueueConsumer"/> class.
	/// </summary>
	/// <param name="connection"></param>
	/// <param name="handler"></param>
	/// <param name="options"></param>
	/// <param name="logger"></param>
	public RabbitMqQueueConsumer(IPersistentConnection connection, IHandlerContext handler, IOptions<RabbitMqMessageBusOptions> options, ILoggerFactory logger)
		: base(connection, options)
	{
		_handler = handler;
		_logger = logger.CreateLogger<RabbitMqQueueConsumer>();
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="RabbitMqQueueConsumer"/> class.
	/// </summary>
	/// <param name="connection"></param>
	/// <param name="handler"></param>
	/// <param name="options"></param>
	/// <param name="logger"></param>
	/// <param name="identity"></param>
	public RabbitMqQueueConsumer(IPersistentConnection connection, IHandlerContext handler, IOptions<RabbitMqMessageBusOptions> options, ILoggerFactory logger, IIdentityProvider identity)
		: this(connection, handler, options, logger)
	{
		_identity = identity;
	}

	/// <inheritdoc />
	public string Name => nameof(RabbitMqQueueConsumer);

	/// <summary>
	/// Gets the RabbitMQ message channel.
	/// </summary>
	private IChannel Channel { get; set; }

	/// <summary>
	/// Gets the RabbitMQ consumer instance.
	/// </summary>
	private AsyncEventingBasicConsumer Consumer { get; set; }

	/// <inheritdoc />
	internal override async Task StartAsync(string channel, CancellationToken cancellationToken = default)
	{
		var queueName = $"{Options.QueueName}${channel}$";
		if (!Connection.IsConnected)
		{
			await Connection.TryConnectAsync();
		}

		Channel = await Connection.CreateChannelAsync();

		await Channel.QueueDeclareAsync(queueName, true, false, false, arguments: null, cancellationToken: cancellationToken);
		await Channel.BasicQosAsync(0, 1, false, cancellationToken);

		Consumer = new AsyncEventingBasicConsumer(Channel);
		Consumer.ReceivedAsync += HandleMessageReceivedAsync;

		await Channel.BasicConsumeAsync(queueName, Options.AutoAck, Consumer, cancellationToken: cancellationToken);
	}

	/// <inheritdoc />
	protected override async Task HandleMessageReceivedAsync(object sender, BasicDeliverEventArgs args)
	{
		var type = MessageTypeCache.GetMessageType(args.BasicProperties.Type);

		var message = DeserializeMessage(args.Body.ToArray(), type);

		var props = args.BasicProperties;

		var context = new MessageContext(message, authorization => _identity?.GetIdentity(authorization));

		await OnMessageReceived(new MessageReceivedEventArgs(message.Data, context));

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

		await HandleAsync(message.Channel, message.Data, context);

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
			var replyProps = new BasicProperties();
			replyProps.Headers ??= new Dictionary<string, object>();
			replyProps.CorrelationId = props.CorrelationId;

			var response = SerializeMessage(reply);
			await Channel.BasicPublishAsync(string.Empty, props.ReplyTo!, true, replyProps, response);
		}

		await Channel.BasicAckAsync(args.DeliveryTag, false);

		OnMessageAcknowledged(new MessageAcknowledgedEventArgs(message.Data, context));
	}

	/// <inheritdoc/>
	protected override async Task HandleAsync(string channel, object message, MessageContext context, CancellationToken cancellationToken = default)
	{
		try
		{
			await _handler.HandleAsync(channel, message, context, cancellationToken);
		}
		catch (Exception exception)
		{
			_logger.LogError(exception, "Message '{Id}' Handle Error: {Message}", context.MessageId, exception.Message);
			context.Failure(exception);
		}
		finally
		{
			context.Complete(null);
		}
	}

	/// <inheritdoc />
	protected override void Dispose(bool disposing)
	{
		if (!disposing)
		{
			return;
		}

		Consumer.ReceivedAsync -= HandleMessageReceivedAsync;
		Channel?.Dispose();
		Connection?.Dispose();
	}
}