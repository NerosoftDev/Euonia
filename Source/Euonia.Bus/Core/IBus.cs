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
		where TMessage : class => PublishAsync(message, new PublishOptions(), cancellationToken);

	/// <summary>
	/// Publishes the specified message.
	/// </summary>
	/// <param name="message"></param>
	/// <param name="options"></param>
	/// <param name="cancellationToken"></param>
	/// <typeparam name="TMessage"></typeparam>
	/// <returns></returns>
	Task PublishAsync<TMessage>(TMessage message, PublishOptions options, CancellationToken cancellationToken = default)
		where TMessage : class;

	/// <summary>
	/// Publishes the specified message.
	/// </summary>
	/// <param name="channel"></param>
	/// <param name="message"></param>
	/// <param name="cancellationToken"></param>
	/// <typeparam name="TMessage"></typeparam>
	/// <returns></returns>
	Task PublishAsync<TMessage>(string channel, TMessage message, CancellationToken cancellationToken = default)
		where TMessage : class => PublishAsync(message, new PublishOptions { Channel = channel }, cancellationToken);

	/// <summary>
	/// Sends the specified message.
	/// </summary>
	/// <param name="message"></param>
	/// <param name="cancellationToken"></param>
	/// <typeparam name="TMessage"></typeparam>
	/// <returns></returns>
	Task SendAsync<TMessage>(TMessage message, CancellationToken cancellationToken = default)
		where TMessage : class => SendAsync(message, new SendOptions(), cancellationToken);

	/// <summary>
	/// Sends the specified message.
	/// </summary>
	/// <param name="message"></param>
	/// <param name="options"></param>
	/// <param name="cancellationToken"></param>
	/// <typeparam name="TMessage"></typeparam>
	/// <returns></returns>
	Task SendAsync<TMessage>(TMessage message, SendOptions options, CancellationToken cancellationToken = default)
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
		where TMessage : class => SendAsync<TMessage, TResult>(message, new SendOptions(), cancellationToken);

	/// <summary>
	/// Sends the specified message.
	/// </summary>
	/// <param name="message"></param>
	/// <param name="options"></param>
	/// <param name="cancellationToken"></param>
	/// <typeparam name="TMessage"></typeparam>
	/// <typeparam name="TResult"></typeparam>
	/// <returns></returns>
	Task<TResult> SendAsync<TMessage, TResult>(TMessage message, SendOptions options, CancellationToken cancellationToken = default)
		where TMessage : class;

	/// <summary>
	/// Sends the specified message.
	/// </summary>
	/// <typeparam name="TResult"></typeparam>
	/// <param name="message"></param>
	/// <param name="cancellationToken"></param>
	/// <returns></returns>
	Task<TResult> SendAsync<TResult>(IQueue<TResult> message, CancellationToken cancellationToken = default) => SendAsync(message, new SendOptions(), cancellationToken);

	/// <summary>
	/// Sends the specified message.
	/// </summary>
	/// <typeparam name="TResult"></typeparam>
	/// <param name="message"></param>
	/// <param name="options"></param>
	/// <param name="cancellationToken"></param>
	/// <returns></returns>
	Task<TResult> SendAsync<TResult>(IQueue<TResult> message, SendOptions options, CancellationToken cancellationToken = default);
}