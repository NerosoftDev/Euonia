namespace Nerosoft.Euonia.Bus;

/// <summary>
/// The implementation of <see cref="IBus"/> interface.
/// </summary>
public sealed class ServiceBus : IBus
{
	private readonly IDispatcher _dispatcher;
	private readonly IMessageConvention _convention;

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

	/// <inheritdoc />
	public async Task PublishAsync<TMessage>(TMessage message, PublishOptions options, CancellationToken cancellationToken = default)
		where TMessage : class
	{
		if (!_convention.IsTopicType(message.GetType()))
		{
			throw new InvalidOperationException("The message type is not an event type.");
		}

		var channelName = options?.Channel ?? MessageCache.Default.GetOrAddChannel<TMessage>();
		var pack = new RoutedMessage<TMessage>(message, channelName)
		{
			MessageId = options?.MessageId ?? Guid.NewGuid().ToString()
		};
		await _dispatcher.PublishAsync(pack, cancellationToken);
	}

	/// <inheritdoc />
	public async Task SendAsync<TMessage>(TMessage message, SendOptions options, CancellationToken cancellationToken = default)
		where TMessage : class
	{
		if (!_convention.IsQueueType(message.GetType()))
		{
			throw new InvalidOperationException("The message type is not a queue type.");
		}

		var channelName = options?.Channel ?? MessageCache.Default.GetOrAddChannel<TMessage>();
		var pack = new RoutedMessage<TMessage>(message, channelName)
		{
			MessageId = options?.MessageId ?? Guid.NewGuid().ToString(),
			CorrelationId = options?.CorrelationId ?? Guid.NewGuid().ToString()
		};
		await _dispatcher.SendAsync(pack, cancellationToken);
	}

	/// <inheritdoc />
	public async Task<TResult> SendAsync<TMessage, TResult>(TMessage message, SendOptions options, CancellationToken cancellationToken = default)
		where TMessage : class
	{
		if (!_convention.IsQueueType(message.GetType()))
		{
			throw new InvalidOperationException("The message type is not a queue type.");
		}

		var channelName = options?.Channel ?? MessageCache.Default.GetOrAddChannel<TMessage>();
		var pack = new RoutedMessage<TMessage, TResult>(message, channelName)
		{
			MessageId = options?.MessageId ?? Guid.NewGuid().ToString(),
			CorrelationId = options?.CorrelationId ?? Guid.NewGuid().ToString()
		};
		return await _dispatcher.SendAsync(pack, cancellationToken);
	}

	/// <inheritdoc />
	public async Task<TResult> SendAsync<TResult>(IQueue<TResult> message, SendOptions options, CancellationToken cancellationToken = default)
	{
		var channelName = options?.Channel ?? MessageCache.Default.GetOrAddChannel(message.GetType());
		var pack = new RoutedMessage<IQueue<TResult>, TResult>(message, channelName)
		{
			MessageId = options?.MessageId ?? Guid.NewGuid().ToString(),
			CorrelationId = options?.CorrelationId ?? Guid.NewGuid().ToString()
		};
		return await _dispatcher.SendAsync(pack, cancellationToken);
	}
}