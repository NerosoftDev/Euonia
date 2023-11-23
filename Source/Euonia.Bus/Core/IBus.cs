namespace Nerosoft.Euonia.Bus;

/// <summary>
/// Interface IBus
/// </summary>
public interface IBus
{
	/// <summary>
	/// Publishes the specified message.
	/// </summary>
	/// <param name="message"></param>
	/// <param name="cancellationToken"></param>
	/// <typeparam name="TMessage"></typeparam>
	/// <returns></returns>
	Task PublishAsync<TMessage>(TMessage message, CancellationToken cancellationToken = default)
		where TMessage : class;

	/// <summary>
	/// Publishes the specified message.
	/// </summary>
	/// <param name="name"></param>
	/// <param name="message"></param>
	/// <param name="cancellationToken"></param>
	/// <typeparam name="TMessage"></typeparam>
	/// <returns></returns>
	Task PublishAsync<TMessage>(string name, TMessage message, CancellationToken cancellationToken = default)
		where TMessage : class;

	/// <summary>
	/// Sends the specified message.
	/// </summary>
	/// <param name="message"></param>
	/// <param name="cancellationToken"></param>
	/// <typeparam name="TMessage"></typeparam>
	/// <returns></returns>
	Task SendAsync<TMessage>(TMessage message, CancellationToken cancellationToken = default)
		where TMessage : class;

	/// <summary>
	/// Sends the specified message.
	/// </summary>
	/// <param name="message"></param>
	/// <param name="cancellationToken"></param>
	/// <typeparam name="TMessage"></typeparam>
	/// <typeparam name="TResult"></typeparam>
	/// <returns></returns>
	Task<TResult> SendAsync<TMessage, TResult>(TMessage message, CancellationToken cancellationToken = default)
		where TMessage : class;
}