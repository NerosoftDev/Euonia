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

	private readonly IDispatcher _dispatcher;
	private readonly IMessageConvention _convention;
	private readonly IRequestContextAccessor _requestAccessor;
	private readonly IServiceProvider _provider;

	/// <summary>
	/// Initializes a new instance of the <see cref="ServiceBus"/> class.
	/// </summary>
	/// <param name="dispatcher"></param>
	/// <param name="provider"></param>
	/// <param name="convention"></param>
	/// <param name="logger"></param>
	public ServiceBus(IDispatcher dispatcher, IServiceProvider provider, IMessageConvention convention, ILoggerFactory logger)
	{
		_logger = logger.CreateLogger<ServiceBus>();
		_dispatcher = dispatcher;
		_convention = convention;
		_provider = provider;
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="ServiceBus"/> class.
	/// </summary>
	/// <param name="dispatcher"></param>
	/// <param name="provider"></param>
	/// <param name="convention"></param>
	/// <param name="logger"></param>
	/// <param name="requestAccessor"></param>
	public ServiceBus(IDispatcher dispatcher, IServiceProvider provider, IMessageConvention convention, ILoggerFactory logger, IRequestContextAccessor requestAccessor)
		: this(dispatcher, provider, convention, logger)
	{
		_requestAccessor = requestAccessor;
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

		var context = _requestAccessor?.Context;

		var channelName = options.Channel ?? MessageCache.Default.GetOrAddChannel(messageType);
		var pack = new RoutedMessage<TMessage>(message, channelName)
		{
			MessageId = options.MessageId ?? ObjectId.NewGuid(GuidType.SequentialAsString).ToString(),
			RequestTraceId = context?.TraceIdentifier ?? options.RequestTraceId ?? ObjectId.NewGuid(GuidType.SequentialAsString).ToString("N"),
			Authorization = context?.Authorization,
		};
		metadataSetter?.Invoke(pack.Metadata);

		var transports = _dispatcher.Determine(messageType);

		var tasks = new List<Task>();

		foreach (var type in transports)
		{
			_logger.LogDebug("Publishing message of type {MessageType} to transport {TransportType} on channel {ChannelName} with MessageId {MessageId}.",
				messageType.FullName, type.FullName, channelName, pack.MessageId);
			var transport = (ITransport)_provider.GetRequiredService(type);
			tasks.Add(transport.PublishAsync(pack, cancellationToken));
		}

		return Task.WhenAll(tasks);
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

		var context = _requestAccessor?.Context;

		var channelName = options.Channel ?? MessageCache.Default.GetOrAddChannel(messageType);
		var pack = new RoutedMessage<TMessage>(message, channelName)
		{
			MessageId = options.MessageId ?? ObjectId.NewGuid(GuidType.SequentialAsString).ToString(),
			CorrelationId = options.CorrelationId ?? ObjectId.NewGuid(GuidType.SequentialAsString).ToString(),
			RequestTraceId = context?.TraceIdentifier ?? options.RequestTraceId ?? ObjectId.NewGuid(GuidType.SequentialAsString).ToString("N"),
			Authorization = context?.Authorization,
		};

		metadataSetter?.Invoke(pack.Metadata);

		var transports = _dispatcher.Determine(messageType);

		var transport = (ITransport)_provider.GetRequiredService(transports.First());

		return transport.SendAsync(pack, cancellationToken)
		                .ContinueWith(task => task.WaitAndUnwrapException(cancellationToken), cancellationToken);
	}

	/// <inheritdoc />
	public async Task<TResult> SendAsync<TMessage, TResult>(TMessage message, SendOptions options, Action<MessageMetadata> metadataSetter = null, Action<TResult> callback = null, CancellationToken cancellationToken = default)
		where TMessage : class
	{
		options ??= new SendOptions();

		var messageType = message.GetType();

		if (!_convention.IsUnicastType(messageType) && !_convention.IsRequestType(messageType))
		{
			throw new MessageTypeException("The message type is not a command type or request type.");
		}

		var context = _requestAccessor?.Context;

		var channelName = options.Channel ?? MessageCache.Default.GetOrAddChannel(messageType);
		var pack = new RoutedMessage<TMessage, TResult>(message, channelName)
		{
			MessageId = options.MessageId ?? ObjectId.NewGuid(GuidType.SequentialAsString).ToString(),
			CorrelationId = options.CorrelationId ?? ObjectId.NewGuid(GuidType.SequentialAsString).ToString(),
			RequestTraceId = context?.TraceIdentifier ?? options.RequestTraceId ?? ObjectId.NewGuid(GuidType.SequentialAsString).ToString("N"),
			Authorization = context?.Authorization,
		};

		metadataSetter?.Invoke(pack.Metadata);

		var transports = _dispatcher.Determine(messageType);

		var transport = (ITransport)_provider.GetRequiredService(transports.First());

		var result = await transport.SendAsync(pack, cancellationToken)
		                            .ContinueWith(task =>
		                            {
			                            task.WaitAndUnwrapException();
			                            var result = task.Result;
			                            callback?.Invoke(result);
			                            return result;
		                            }, cancellationToken);
		return result;
	}

	/// <inheritdoc />
	public async Task<TResult> RequestAsync<TResult>(IRequest<TResult> message, SendOptions options, Action<MessageMetadata> metadataSetter = null, Action<TResult> callback = null, CancellationToken cancellationToken = default)
	{
		options ??= new SendOptions();

		var messageType = message.GetType();

		if (!_convention.IsRequestType(messageType))
		{
			throw new MessageTypeException("The message type is not a queue type.");
		}

		var context = _requestAccessor?.Context;

		var channelName = options.Channel ?? MessageCache.Default.GetOrAddChannel(messageType);
		var pack = new RoutedMessage<IRequest<TResult>, TResult>(message, channelName)
		{
			MessageId = options.MessageId ?? ObjectId.NewGuid(GuidType.SequentialAsString).ToString(),
			CorrelationId = options.CorrelationId ?? ObjectId.NewGuid(GuidType.SequentialAsString).ToString(),
			RequestTraceId = context?.TraceIdentifier ?? options.RequestTraceId ?? ObjectId.NewGuid(GuidType.SequentialAsString).ToString("N"),
			Authorization = context?.Authorization,
		};

		metadataSetter?.Invoke(pack.Metadata);

		var transports = _dispatcher.Determine(messageType);

		var transport = (ITransport)_provider.GetRequiredService(transports.First());

		var result = await transport.SendAsync(pack, cancellationToken)
		                            .ContinueWith(task =>
		                            {
			                            task.WaitAndUnwrapException();
			                            var result = task.Result;
			                            callback?.Invoke(result);
			                            return result;
		                            }, cancellationToken);
		return result;
	}
}