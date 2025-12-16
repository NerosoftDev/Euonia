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
		where TMessage : class => PublishAsync(message, new PublishOptions(), null, cancellationToken);

	/// <summary>
	/// Publishes the specified message.
	/// </summary>
	/// <param name="message"></param>
	/// <param name="options"></param>
	/// <param name="metadataSetter"></param>
	/// <param name="cancellationToken"></param>
	/// <typeparam name="TMessage"></typeparam>
	/// <returns></returns>
	Task PublishAsync<TMessage>(TMessage message, PublishOptions options, Action<MessageMetadata> metadataSetter = null, CancellationToken cancellationToken = default)
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
		where TMessage : class => PublishAsync(channel, message, null, cancellationToken);

	/// <summary>
	/// Publishes the specified message.
	/// </summary>
	/// <typeparam name="TMessage"></typeparam>
	/// <param name="channel"></param>
	/// <param name="message"></param>
	/// <param name="metadataSetter"></param>
	/// <param name="cancellationToken"></param>
	/// <returns></returns>
	Task PublishAsync<TMessage>(string channel, TMessage message, Action<MessageMetadata> metadataSetter = null, CancellationToken cancellationToken = default)
		where TMessage : class => PublishAsync(message, new PublishOptions { Channel = channel }, metadataSetter, cancellationToken);

	/// <summary>
	/// Sends the specified message.
	/// </summary>
	/// <param name="message"></param>
	/// <param name="cancellationToken"></param>
	/// <typeparam name="TMessage"></typeparam>
	/// <returns></returns>
	Task SendAsync<TMessage>(TMessage message, CancellationToken cancellationToken = default)
		where TMessage : class => SendAsync(message, new SendOptions(), null, cancellationToken);

	/// <summary>
	/// Sends the specified message.
	/// </summary>
	/// <typeparam name="TMessage"></typeparam>
	/// <param name="message"></param>
	/// <param name="options"></param>
	/// <param name="metadataSetter"></param>
	/// <param name="cancellationToken"></param>
	/// <returns></returns>
	Task SendAsync<TMessage>(TMessage message, SendOptions options, Action<MessageMetadata> metadataSetter = null, CancellationToken cancellationToken = default)
		where TMessage : class => SendAsync<TMessage, Unit>(message, options, metadataSetter, null, cancellationToken);

	/// <summary>
	/// Sends the specified message.
	/// </summary>
	/// <param name="message"></param>
	/// <param name="cancellationToken"></param>
	/// <typeparam name="TMessage"></typeparam>
	/// <typeparam name="TResult"></typeparam>
	/// <returns></returns>
	Task<TResult> SendAsync<TMessage, TResult>(TMessage message, CancellationToken cancellationToken = default)
		where TMessage : class => SendAsync<TMessage, TResult>(message, new SendOptions(), null, null, cancellationToken);

	/// <summary>
	/// Sends the specified message.
	/// </summary>
	/// <typeparam name="TMessage"></typeparam>
	/// <typeparam name="TResult"></typeparam>
	/// <param name="message"></param>
	/// <param name="options"></param>
	/// <param name="metadataSetter"></param>
	/// <param name="callback"></param>
	/// <param name="cancellationToken"></param>
	/// <returns></returns>
	Task<TResult> SendAsync<TMessage, TResult>(TMessage message, SendOptions options, Action<MessageMetadata> metadataSetter = null, Action<TResult> callback = null, CancellationToken cancellationToken = default)
		where TMessage : class;

	/// <summary>
	/// Sends the specified message.
	/// </summary>
	/// <typeparam name="TResult"></typeparam>
	/// <param name="message"></param>
	/// <param name="cancellationToken"></param>
	/// <returns></returns>
	Task<TResult> CallAsync<TResult>(IRequest<TResult> message, CancellationToken cancellationToken = default) => CallAsync(message, new SendOptions(), null, cancellationToken);

	/// <summary>
	/// Sends the specified message.
	/// </summary>
	/// <typeparam name="TResult"></typeparam>
	/// <param name="message"></param>
	/// <param name="options"></param>
	/// <param name="metadataSetter"></param>
	/// <param name="cancellationToken"></param>
	/// <returns></returns>
	Task<TResult> CallAsync<TResult>(IRequest<TResult> message, SendOptions options, Action<MessageMetadata> metadataSetter = null, CancellationToken cancellationToken = default);
}