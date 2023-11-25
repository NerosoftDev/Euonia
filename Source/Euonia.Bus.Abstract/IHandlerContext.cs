namespace Nerosoft.Euonia.Bus;

/// <summary>
/// Specifies contract of message handler context.
/// </summary>
public interface IHandlerContext
{
	/// <summary>
	/// Occurs when message subscribed.
	/// </summary>
	event EventHandler<MessageSubscribedEventArgs> MessageSubscribed;

	/// <summary>
	/// Handle message asynchronously.
	/// </summary>
	/// <param name="message">The message to be handled.</param>
	/// <param name="context">The message context.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>Task.</returns>
	Task HandleAsync(object message, MessageContext context, CancellationToken cancellationToken = default);

	/// <summary>
	/// Handle message asynchronously.
	/// </summary>
	/// <param name="name"></param>
	/// <param name="message"></param>
	/// <param name="context"></param>
	/// <param name="cancellationToken"></param>
	/// <returns></returns>
	Task HandleAsync(string name, object message, MessageContext context, CancellationToken cancellationToken = default);
}