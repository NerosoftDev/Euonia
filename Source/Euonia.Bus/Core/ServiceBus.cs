using Microsoft.Extensions.DependencyInjection;

namespace Nerosoft.Euonia.Bus;

/// <summary>
/// The implementation of <see cref="IBus"/> interface.
/// </summary>
public sealed class ServiceBus : IBus
{
	private readonly IDispatcher _dispatcher;
	private readonly IMessageConvention _convention;
	private readonly IServiceAccessor _serviceAccessor;
	private readonly RequestContextAccessor _contextAccessor;

	/// <summary>
	/// Initializes a new instance of the <see cref="ServiceBus"/> class.
	/// </summary>
	/// <param name="factory"></param>
	/// <param name="convention"></param>
	public ServiceBus(IBusFactory factory, IMessageConvention convention)
	{
		_dispatcher = factory.CreateDispatcher();
		_convention = convention;
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="ServiceBus"/> class.
	/// </summary>
	/// <param name="factory"></param>
	/// <param name="convention"></param>
	/// <param name="serviceAccessor"></param>
	public ServiceBus(IBusFactory factory, IMessageConvention convention, IServiceAccessor serviceAccessor)
		: this(factory, convention)
	{
		_serviceAccessor = serviceAccessor;
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="ServiceBus"/> class.
	/// </summary>
	/// <param name="factory"></param>
	/// <param name="convention"></param>
	/// <param name="serviceAccessor"></param>
	/// <param name="contextAccessor"></param>
	public ServiceBus(IBusFactory factory, IMessageConvention convention, IServiceAccessor serviceAccessor, RequestContextAccessor contextAccessor)
		: this(factory, convention, serviceAccessor)
	{
		_contextAccessor = contextAccessor;
	}

	/// <inheritdoc />
	public Task PublishAsync<TMessage>(TMessage message, PublishOptions options, Action<MessageMetadata> metadataSetter = null, CancellationToken cancellationToken = default)
		where TMessage : class
	{
		if (!_convention.IsTopicType(message.GetType()))
		{
			throw new InvalidOperationException("The message type is not an event type.");
		}

		var context = GetRequestContext();

		var channelName = options?.Channel ?? MessageCache.Default.GetOrAddChannel<TMessage>();
		var pack = new RoutedMessage<TMessage>(message, channelName)
		{
			MessageId = options?.MessageId ?? Guid.NewGuid().ToString(),
			RequestTraceId = context?.TraceIdentifier ?? options.RequestTraceId ?? Guid.NewGuid().ToString("N"),
		};
		metadataSetter?.Invoke(pack.Metadata);
		return _dispatcher.PublishAsync(pack, cancellationToken);
	}

	/// <inheritdoc />
	public Task SendAsync<TMessage>(TMessage message, SendOptions options, Action<MessageMetadata> metadataSetter = null, CancellationToken cancellationToken = default)
		where TMessage : class
	{
		if (!_convention.IsQueueType(message.GetType()))
		{
			throw new InvalidOperationException("The message type is not a queue type.");
		}

		var context = GetRequestContext();

		var channelName = options?.Channel ?? MessageCache.Default.GetOrAddChannel<TMessage>();
		var pack = new RoutedMessage<TMessage>(message, channelName)
		{
			MessageId = options?.MessageId ?? Guid.NewGuid().ToString(),
			CorrelationId = options?.CorrelationId ?? Guid.NewGuid().ToString(),
			RequestTraceId = context?.TraceIdentifier ?? options.RequestTraceId ?? Guid.NewGuid().ToString("N"),
			Authorization = context?.Authorization,
		};

		metadataSetter?.Invoke(pack.Metadata);

		return _dispatcher.SendAsync(pack, cancellationToken).ContinueWith(task => task.WaitAndUnwrapException());
	}

	/// <inheritdoc />
	public Task<TResult> SendAsync<TMessage, TResult>(TMessage message, SendOptions options, Action<MessageMetadata> metadataSetter = null, Action<TResult> callback = null, CancellationToken cancellationToken = default)
		where TMessage : class
	{
		if (!_convention.IsQueueType(message.GetType()))
		{
			throw new InvalidOperationException("The message type is not a queue type.");
		}

		var context = GetRequestContext();

		var channelName = options?.Channel ?? MessageCache.Default.GetOrAddChannel<TMessage>();
		var pack = new RoutedMessage<TMessage, TResult>(message, channelName)
		{
			MessageId = options?.MessageId ?? Guid.NewGuid().ToString(),
			CorrelationId = options?.CorrelationId ?? Guid.NewGuid().ToString(),
			RequestTraceId = context?.TraceIdentifier ?? options.RequestTraceId ?? Guid.NewGuid().ToString("N"),
			Authorization = context?.Authorization,
		};

		metadataSetter?.Invoke(pack.Metadata);

		return _dispatcher.SendAsync(pack, cancellationToken)
						  .ContinueWith(task =>
						  {
							  task.WaitAndUnwrapException();
							  var result = task.Result;
							  callback?.Invoke(result);
							  return result;
						  });
	}

	/// <inheritdoc />
	public Task<TResult> SendAsync<TResult>(IQueue<TResult> message, SendOptions options, Action<MessageMetadata> metadataSetter = null, Action<TResult> callback = null, CancellationToken cancellationToken = default)
	{
		var context = GetRequestContext();

		var channelName = options?.Channel ?? MessageCache.Default.GetOrAddChannel(message.GetType());
		var pack = new RoutedMessage<IQueue<TResult>, TResult>(message, channelName)
		{
			MessageId = options?.MessageId ?? Guid.NewGuid().ToString(),
			CorrelationId = options?.CorrelationId ?? Guid.NewGuid().ToString(),
			RequestTraceId = context?.TraceIdentifier ?? options.RequestTraceId ?? Guid.NewGuid().ToString("N"),
			Authorization = context?.Authorization,
		};

		metadataSetter?.Invoke(pack.Metadata);

		return _dispatcher.SendAsync(pack, cancellationToken)
						  .ContinueWith(task =>
						  {
							  task.WaitAndUnwrapException();
							  var result = task.Result;
							  callback?.Invoke(result);
							  return result;
						  });
	}

	private RequestContext GetRequestContext()
	{
		if (_contextAccessor == null)
		{
			return null;
		}

		var context = _contextAccessor(_serviceAccessor.ServiceProvider);
		return context;
	}
}