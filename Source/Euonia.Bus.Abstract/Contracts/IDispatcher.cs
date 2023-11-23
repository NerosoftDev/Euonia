namespace Nerosoft.Euonia.Bus;

/// <summary>
/// 
/// </summary>
public interface IDispatcher
{
	/// <summary>
	/// Occurs when [message dispatched].
	/// </summary>
	event EventHandler<MessageDispatchedEventArgs> Delivered;

	/// <summary>
	/// Publishes the specified message.
	/// </summary>
	/// <param name="pack"></param>
	/// <param name="cancellationToken"></param>
	/// <typeparam name="TMessage"></typeparam>
	/// <returns></returns>
	Task PublishAsync<TMessage>(RoutedMessage<TMessage> pack, CancellationToken cancellationToken = default)
		where TMessage : class;

	/// <summary>
	/// Sends the specified message.
	/// </summary>
	/// <param name="pack"></param>
	/// <param name="cancellationToken"></param>
	/// <typeparam name="TMessage"></typeparam>
	/// <returns></returns>
	Task SendAsync<TMessage>(RoutedMessage<TMessage> pack, CancellationToken cancellationToken = default)
		where TMessage : class;

	/// <summary>
	/// Sends the specified message.
	/// </summary>
	/// <param name="pack"></param>
	/// <param name="cancellationToken"></param>
	/// <typeparam name="TMessage"></typeparam>
	/// <typeparam name="TResult"></typeparam>
	/// <returns></returns>
	Task<TResult> SendAsync<TMessage, TResult>(RoutedMessage<TMessage> pack, CancellationToken cancellationToken = default)
		where TMessage : class;
}