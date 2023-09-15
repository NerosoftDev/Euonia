using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nerosoft.Euonia.Domain;
using Polly;
using RabbitMQ.Client;

namespace Nerosoft.Euonia.Bus.RabbitMq;

/// <summary>
/// The event bus implementation that uses RabbitMQ for publishing and subscribing to events.
/// </summary>
public class EventBus : MessageBus, IEventBus
{
	private readonly ConnectionFactory _factory;
	private readonly IEventStore _eventStore;
	private readonly IConnection _connection;
	private readonly IModel _channel;
	private readonly ILogger<EventBus> _logger;
	private bool _disposed;

	private static readonly ConcurrentDictionary<string, EventConsumer> _consumers = new();

	/// <summary>
	/// Initialize a new instance of <see cref="EventBus"/>.
	/// </summary>
	/// <param name="handlerContext"></param>
	/// <param name="monitor"></param>
	/// <param name="accessor"></param>
	/// <param name="logger"></param>
	/// <exception cref="ArgumentNullException"></exception>
	public EventBus(IMessageHandlerContext handlerContext, IOptionsMonitor<RabbitMqMessageBusOptions> monitor, IServiceAccessor accessor, ILoggerFactory logger)
		: base(handlerContext, monitor, accessor)
	{
		_logger = logger.CreateLogger<EventBus>();
		_factory = new ConnectionFactory { Uri = new Uri(Options.Connection) };
		_connection = _factory.CreateConnection();
		_channel = _connection.CreateModel();

		if (string.IsNullOrEmpty(Options.ExchangeName))
		{
			throw new ArgumentNullException(nameof(Options.ExchangeName), Resources.IDS_EXCHANGE_NAME_IS_REQUIRED);
		}

		if (string.IsNullOrEmpty(Options.ExchangeType))
		{
			throw new ArgumentNullException(nameof(Options.ExchangeType), Resources.IDS_EXCHANGE_TYPE_IS_REQUIRED);
		}

		// Declares the exchange
		_channel.ExchangeDeclare(Options.ExchangeName, Options.ExchangeType);

		HandlerContext.MessageSubscribed += HandleMessageSubscribed;
	}

	/// <summary>
	/// Initialize a new instance of <see cref="EventBus"/>.
	/// </summary>
	/// <param name="handlerContext"></param>
	/// <param name="monitor"></param>
	/// <param name="accessor"></param>
	/// <param name="eventStore"></param>
	/// <param name="logger"></param>
	public EventBus(IMessageHandlerContext handlerContext, IOptionsMonitor<RabbitMqMessageBusOptions> monitor, IServiceAccessor accessor, IEventStore eventStore, ILoggerFactory logger)
		: this(handlerContext, monitor, accessor, logger)
	{
		_eventStore = eventStore;
	}

	private void HandleMessageSubscribed(object sender, MessageSubscribedEventArgs args)
	{
		// ReSharper disable once HeapView.CanAvoidClosure
		_consumers.GetOrAdd(args.MessageName, name =>
		{
			var consumer = new EventConsumer(_factory, Options, HandlerContext, name)
			{
				OnMessageAcknowledged = OnMessageAcknowledged,
				OnMessageReceived = OnMessageReceived
			};
			return consumer;
		});

		OnMessageSubscribed(args);
	}

	/// <inheritdoc />
	public async Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
		where TEvent : IEvent
	{
		if (_eventStore != null)
		{
			await _eventStore.SaveAsync(@event, cancellationToken);
		}

		var messageContext = new MessageContext();

		await Task.Run(() =>
		{
			try
			{
				var messageBody = Serialize(@event);
				var props = _channel.CreateBasicProperties();
				props.Headers ??= new Dictionary<string, object>();
				if (@event.HasAttribute(out EventNameAttribute attribute, false))
				{
					props.Headers[Constants.MessageHeaderEventAttr] = attribute.Name; //Encoding.UTF8.GetBytes(attribute.Name);
				}
				else
				{
					props.Headers[Constants.MessageHeaderEventType] = @event.Metadata[Message.MessageTypeKey]; //Encoding.UTF8.GetBytes((@event.Metadata[MessageBase.MESSAGE_TYPE_KEY] as string)!);
				}

				Policy.Handle<Exception>()
				      .WaitAndRetry(Options.MaxFailureRetries, _ => TimeSpan.FromSeconds(3), (exception, _, retryCount, _) =>
				      {
					      _logger.LogError(exception, "Retry:{RetryCount}, {Message}", retryCount, exception.Message);
				      })
				      .Execute(() =>
				      {
					      _channel.BasicPublish(Options.ExchangeName, @event.GetType().FullName, props, messageBody);
				      });
			}
			catch (Exception exception)
			{
				_logger.LogError(exception, "Message publish failed:{Message}", exception.Message);
				throw;
			}
		}, cancellationToken);

		OnMessageDispatched(new MessageDispatchedEventArgs(@event, messageContext));
	}

	/// <inheritdoc />
	public async Task PublishAsync<TEvent>(string name, TEvent @event, CancellationToken cancellationToken = default)
		where TEvent : class
	{
		var namedEvent = new NamedEvent(name, @event);
		if (_eventStore != null)
		{
			await _eventStore.SaveAsync(namedEvent, cancellationToken);
		}

		var messageContext = new MessageContext();
		await Task.Run(() =>
		{
			try
			{
				var messageBody = Serialize(@event);
				var props = _channel.CreateBasicProperties();
				props.Headers ??= new Dictionary<string, object>();
				props.Headers[Constants.MessageHeaderEventName] = name;

				Policy.Handle<Exception>()
				      .WaitAndRetry(Options.MaxFailureRetries, _ => TimeSpan.FromSeconds(3), (exception, _, retryCount, _) =>
				      {
					      _logger.LogError(exception, "Retry:{RetryCount}, {Message}", retryCount, exception.Message);
				      })
				      .Execute(() =>
				      {
					      _channel.BasicPublish(Options.ExchangeName, @event.GetType().FullName, props, messageBody);
				      });
			}
			catch (Exception exception)
			{
				_logger.LogError(exception, "Message publish failed:{Message}", exception.Message);
				throw;
			}
		}, cancellationToken);

		OnMessageDispatched(new MessageDispatchedEventArgs(namedEvent, messageContext));
	}

	/// <inheritdoc />
	public void Subscribe<TEvent, THandler>()
		where TEvent : IEvent
		where THandler : IEventHandler<TEvent>
	{
		HandlerContext.Register<TEvent, THandler>();
	}

	/// <summary>
	/// Releases unmanaged and - optionally - managed resources.
	/// </summary>
	/// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
	protected override void Dispose(bool disposing)
	{
		_logger.LogInformation("EventBus disposing...");
		if (_disposed)
		{
			return;
		}

		if (disposing)
		{
			_channel?.Dispose();
			_connection?.Dispose();
		}

		_disposed = true;
	}
}