using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Nerosoft.Euonia.Modularity;

namespace Nerosoft.Euonia.Bus;

/// <summary>
/// The implementation of <see cref="IBus"/> interface.
/// </summary>
public sealed class ServiceBus : IBus
{
	private readonly ILogger<ServiceBus> _logger;

	private readonly ITransport _transport;
	private readonly IMessageConvention _convention;
	private readonly IServiceAccessor _serviceAccessor;

	/// <summary>
	/// Initializes a new instance of the <see cref="ServiceBus"/> class.
	/// </summary>
	/// <param name="factory"></param>
	/// <param name="convention"></param>
	/// <param name="logger"></param>
	public ServiceBus(IBusFactory factory, IMessageConvention convention, ILoggerFactory logger)
	{
		_logger = logger.CreateLogger<ServiceBus>();
		_transport = factory.CreateDispatcher();
		_convention = convention;
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="ServiceBus"/> class.
	/// </summary>
	/// <param name="factory"></param>
	/// <param name="convention"></param>
	/// <param name="logger"></param>
	/// <param name="serviceAccessor"></param>
	public ServiceBus(IBusFactory factory, IMessageConvention convention, ILoggerFactory logger, IServiceAccessor serviceAccessor)
		: this(factory, convention, logger)
	{
		_serviceAccessor = serviceAccessor;
	}

	/// <inheritdoc />
	public Task PublishAsync<TMessage>(TMessage message, PublishOptions options, Action<MessageMetadata> metadataSetter = null, CancellationToken cancellationToken = default)
		where TMessage : class
	{
		options ??= new PublishOptions();

		var messageType = message.GetType();

		if (!_convention.IsMulticastType(messageType))
		{
			throw new MessageTypeException("The message type is not an topic type.");
		}

		var context = GetRequestContext();

		var channelName = options.Channel ?? MessageCache.Default.GetOrAddChannel(messageType);
		var pack = new RoutedMessage<TMessage>(message, channelName)
		{
			MessageId = options.MessageId ?? ObjectId.NewGuid(GuidType.SequentialAsString).ToString(),
			RequestTraceId = context?.TraceIdentifier ?? options.RequestTraceId ?? ObjectId.NewGuid(GuidType.SequentialAsString).ToString("N"),
			Authorization = context?.Authorization,
		};
		metadataSetter?.Invoke(pack.Metadata);
		return _transport.PublishAsync(pack, cancellationToken);
	}

	/// <inheritdoc />
	public Task SendAsync<TMessage>(TMessage message, SendOptions options, Action<MessageMetadata> metadataSetter = null, CancellationToken cancellationToken = default)
		where TMessage : class
	{
		options ??= new SendOptions();

		var messageType = message.GetType();

		if (!_convention.IsUnicastType(messageType))
		{
			throw new MessageTypeException("The message type is not a command type.");
		}

		var context = GetRequestContext();

		var channelName = options.Channel ?? MessageCache.Default.GetOrAddChannel(messageType);
		var pack = new RoutedMessage<TMessage>(message, channelName)
		{
			MessageId = options.MessageId ?? ObjectId.NewGuid(GuidType.SequentialAsString).ToString(),
			CorrelationId = options.CorrelationId ?? ObjectId.NewGuid(GuidType.SequentialAsString).ToString(),
			RequestTraceId = context?.TraceIdentifier ?? options.RequestTraceId ?? ObjectId.NewGuid(GuidType.SequentialAsString).ToString("N"),
			Authorization = context?.Authorization,
		};

		metadataSetter?.Invoke(pack.Metadata);

		return _transport.SendAsync(pack, cancellationToken)
		                  .ContinueWith(task => task.WaitAndUnwrapException(cancellationToken), cancellationToken);
	}

	/// <inheritdoc />
	public Task<TResult> SendAsync<TMessage, TResult>(TMessage message, SendOptions options, Action<MessageMetadata> metadataSetter = null, Action<TResult> callback = null, CancellationToken cancellationToken = default)
		where TMessage : class
	{
		options ??= new SendOptions();

		var messageType = message.GetType();

		if (!_convention.IsUnicastType(messageType) && !_convention.IsRequestType(messageType))
		{
			throw new MessageTypeException("The message type is not a command type or request type.");
		}

		var context = GetRequestContext();

		var channelName = options.Channel ?? MessageCache.Default.GetOrAddChannel(messageType);
		var pack = new RoutedMessage<TMessage, TResult>(message, channelName)
		{
			MessageId = options.MessageId ?? ObjectId.NewGuid(GuidType.SequentialAsString).ToString(),
			CorrelationId = options.CorrelationId ?? ObjectId.NewGuid(GuidType.SequentialAsString).ToString(),
			RequestTraceId = context?.TraceIdentifier ?? options.RequestTraceId ?? ObjectId.NewGuid(GuidType.SequentialAsString).ToString("N"),
			Authorization = context?.Authorization,
		};

		metadataSetter?.Invoke(pack.Metadata);

		return _transport.SendAsync(pack, cancellationToken)
		                  .ContinueWith(task =>
		                  {
			                  task.WaitAndUnwrapException();
			                  var result = task.Result;
			                  callback?.Invoke(result);
			                  return result;
		                  }, cancellationToken);
	}

	/// <inheritdoc />
	public Task<TResult> RequestAsync<TResult>(IRequest<TResult> message, SendOptions options, Action<MessageMetadata> metadataSetter = null, Action<TResult> callback = null, CancellationToken cancellationToken = default)
	{
		options ??= new SendOptions();

		var messageType = message.GetType();

		if (!_convention.IsRequestType(messageType))
		{
			throw new MessageTypeException("The message type is not a queue type.");
		}

		var context = GetRequestContext();

		var channelName = options.Channel ?? MessageCache.Default.GetOrAddChannel(messageType);
		var pack = new RoutedMessage<IRequest<TResult>, TResult>(message, channelName)
		{
			MessageId = options.MessageId ?? ObjectId.NewGuid(GuidType.SequentialAsString).ToString(),
			CorrelationId = options.CorrelationId ?? ObjectId.NewGuid(GuidType.SequentialAsString).ToString(),
			RequestTraceId = context?.TraceIdentifier ?? options.RequestTraceId ?? ObjectId.NewGuid(GuidType.SequentialAsString).ToString("N"),
			Authorization = context?.Authorization,
		};

		metadataSetter?.Invoke(pack.Metadata);

		return _transport.SendAsync(pack, cancellationToken)
		                 .ContinueWith(task =>
		                 {
			                 task.WaitAndUnwrapException();
			                 var result = task.Result;
			                 callback?.Invoke(result);
			                 return result;
		                 }, cancellationToken);
	}

	private RequestContext GetRequestContext()
	{
		if (_serviceAccessor == null)
		{
			_logger.LogWarning("The IServiceAccessor is not available in the ServiceBus.");
			return null;
		}

		if (_serviceAccessor.ServiceProvider == null)
		{
			_logger.LogWarning("The IServiceProvider is not available in the ServiceBus.");
			return null;
		}

		var accessor = _serviceAccessor.ServiceProvider.GetService<IRequestContextAccessor>();
		
		if (accessor == null)
		{
			_logger.LogWarning("The IRequestContextAccessor is not registered in the IoC container.");
			return null;
		}

		{
		}

		return accessor.Context;
	}
}