namespace Nerosoft.Euonia.Bus;

/// <summary>
/// 
/// </summary>
public interface IDispatcher
{
	/// <summary>
	/// Occurs when [message dispatched].
	/// </summary>
	event EventHandler<MessageDeliveredEventArgs> Delivered;

	/// <summary>
	/// Publishes the specified message.
	/// </summary>
	/// <param name="message"></param>
	/// <param name="cancellationToken"></param>
	/// <typeparam name="TMessage"></typeparam>
	/// <returns></returns>
	Task PublishAsync<TMessage>(RoutedMessage<TMessage> message, CancellationToken cancellationToken = default)
		where TMessage : class;

	/// <summary>
	/// Sends the specified message.
	/// </summary>
	/// <param name="message"></param>
	/// <param name="cancellationToken"></param>
	/// <typeparam name="TMessage"></typeparam>
	/// <returns></returns>
	Task SendAsync<TMessage>(RoutedMessage<TMessage> message, CancellationToken cancellationToken = default)
		where TMessage : class;

	/// <summary>
	/// Sends the specified message.
	/// </summary>
	/// <param name="message"></param>
	/// <param name="cancellationToken"></param>
	/// <typeparam name="TMessage"></typeparam>
	/// <typeparam name="TResponse"></typeparam>
	/// <returns></returns>
	Task<TResponse> SendAsync<TMessage, TResponse>(RoutedMessage<TMessage, TResponse> message, CancellationToken cancellationToken = default)
		where TMessage : class;
}