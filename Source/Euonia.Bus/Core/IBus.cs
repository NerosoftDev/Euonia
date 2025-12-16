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
		where TMessage : class => SendAsync<TMessage, Unit>(message, null, options, metadataSetter, cancellationToken);

	/// <summary>
	/// Sends the specified message.
	/// </summary>
	/// <param name="message"></param>
	/// <param name="callback"></param>
	/// <param name="cancellationToken"></param>
	/// <typeparam name="TMessage"></typeparam>
	/// <typeparam name="TResult"></typeparam>
	/// <returns></returns>
	Task SendAsync<TMessage, TResult>(TMessage message, Action<TResult> callback, CancellationToken cancellationToken = default)
		where TMessage : class => SendAsync(message, callback, new SendOptions(), null, cancellationToken);

	/// <summary>
	/// Sends the specified message.
	/// </summary>
	/// <typeparam name="TMessage"></typeparam>
	/// <typeparam name="TResult"></typeparam>
	/// <param name="message"></param>
	/// <param name="callback"></param>
	/// <param name="options"></param>
	/// <param name="metadataSetter"></param>
	/// <param name="cancellationToken"></param>
	/// <returns></returns>
	Task SendAsync<TMessage, TResult>(TMessage message, Action<TResult> callback, SendOptions options, Action<MessageMetadata> metadataSetter = null, CancellationToken cancellationToken = default)
		where TMessage : class;

	/// <summary>
	/// Sends the specified message.
	/// </summary>
	/// <typeparam name="TResult"></typeparam>
	/// <param name="message"></param>
	/// <param name="cancellationToken"></param>
	/// <returns></returns>
	Task<TResult> CallAsync<TResult>(IRequest<TResult> message, CancellationToken cancellationToken = default) => CallAsync(message, new CallOptions(), null, cancellationToken);

	/// <summary>
	/// Sends the specified message.
	/// </summary>
	/// <typeparam name="TResult"></typeparam>
	/// <param name="message"></param>
	/// <param name="options"></param>
	/// <param name="metadataSetter"></param>
	/// <param name="cancellationToken"></param>
	/// <returns></returns>
	Task<TResult> CallAsync<TResult>(IRequest<TResult> message, CallOptions options, Action<MessageMetadata> metadataSetter = null, CancellationToken cancellationToken = default);
}