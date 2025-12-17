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

		foreach (var name in transports)
		{
			_logger.LogDebug("Publishing message of type {MessageType} to transport {TransportType} on channel {ChannelName} with MessageId {MessageId}.",
				messageType.FullName, name, channelName, pack.MessageId);
			var transport = _provider.GetKeyedService<ITransport>(name);
			tasks.Add(transport.PublishAsync(pack, cancellationToken));
		}

		return Task.WhenAll(tasks);
	}

	/// <inheritdoc />
	public async Task SendAsync<TMessage, TResult>(TMessage message, Action<TResult> callback = null, SendOptions options = null, Action<MessageMetadata> metadataSetter = null, CancellationToken cancellationToken = default)
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

		var transport = _provider.GetKeyedService<ITransport>(transports.First());

		await transport.SendAsync(pack, cancellationToken)
		               .ContinueWith(task =>
		               {
			               task.WaitAndUnwrapException();
			               var result = task.Result;
			               callback?.Invoke(result);
		               }, cancellationToken);
	}

	/// <inheritdoc />
	public async Task<TResult> CallAsync<TResult>(IRequest<TResult> message, CallOptions options, Action<MessageMetadata> metadataSetter = null, CancellationToken cancellationToken = default)
	{
		options ??= new CallOptions();

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

		var transport = _provider.GetKeyedService<ITransport>(transports.First());

		var result = await transport.SendAsync(pack, cancellationToken)
		                            .ContinueWith(task =>
		                            {
			                            task.WaitAndUnwrapException();
			                            return task.Result;
		                            }, cancellationToken);
		return result;
	}
}