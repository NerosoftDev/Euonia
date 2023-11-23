using System.Reflection;

namespace Nerosoft.Euonia.Bus;

/// <summary>
/// 
/// </summary>
public sealed class ServiceBus : IBus
{
	private readonly IDispatcher _dispatcher;
	private readonly MessageConvention _convention;

	/// <summary>
	/// Initialize a new instance of <see cref="ServiceBus"/>
	/// </summary>
	/// <param name="factory"></param>
	/// <param name="convention"></param>
	public ServiceBus(IBusFactory factory, MessageConvention convention)
	{
		_convention = convention;
		_dispatcher = factory.CreateDispatcher();
	}

	/// <inheritdoc />
	public async Task PublishAsync<TMessage>(TMessage message, CancellationToken cancellationToken = default)
		where TMessage : class
	{
		if (!_convention.IsEventType(message.GetType()))
		{
			throw new InvalidOperationException("The message type is not an event type.");
		}

		var pack = new RoutedMessage<TMessage>(message, typeof(void).FullName);
		await _dispatcher.PublishAsync(pack, cancellationToken);
	}

	/// <inheritdoc />
	public async Task PublishAsync<TMessage>(string name, TMessage message, CancellationToken cancellationToken = default)
		where TMessage : class
	{
		if (!_convention.IsEventType(message.GetType()))
		{
			throw new InvalidOperationException("The message type is not an event type.");
		}

		var pack = new RoutedMessage<TMessage>(message, name);
		await _dispatcher.PublishAsync(pack, cancellationToken);
	}

	/// <inheritdoc />
	public async Task SendAsync<TMessage>(TMessage message, CancellationToken cancellationToken = default)
		where TMessage : class
	{
		if (!_convention.IsCommandType(message.GetType()))
		{
			throw new InvalidOperationException("The message type is not a command type.");
		}

		var pack = new RoutedMessage<TMessage>(message, typeof(void).FullName);
		await _dispatcher.SendAsync(pack, cancellationToken);
	}

	/// <inheritdoc />
	public async Task<TResult> SendAsync<TMessage, TResult>(TMessage message, CancellationToken cancellationToken = default)
		where TMessage : class
	{
		if (!_convention.IsCommandType(message.GetType()))
		{
			throw new InvalidOperationException("The message type is not a command type.");
		}

		var pack = new RoutedMessage<TMessage>(message, GetChannelName<TMessage>());
		await _dispatcher.SendAsync(pack, cancellationToken);
		return default;
	}

	private static string GetChannelName<TMessage>()
	{
		var attribute = typeof(TMessage).GetCustomAttribute<ChannelAttribute>();
		return attribute?.Name ?? typeof(TMessage).Name;
	}
}