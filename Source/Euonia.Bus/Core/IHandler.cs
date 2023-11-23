namespace Nerosoft.Euonia.Bus;

/// <summary>
/// Contract of message handler.
/// </summary>
public interface IHandler
{
	/// <summary>
	/// Determines whether the current message handler can handle the message with the specified message type.
	/// </summary>
	/// <param name="messageType">Type of the message to be checked.</param>
	/// <returns><c>true</c> if the current message handler can handle the message with the specified message type; otherwise, <c>false</c>.</returns>
	bool CanHandle(Type messageType);
	
	/// <summary>
	/// Handle message.
	/// </summary>
	/// <param name="message"></param>
	/// <param name="messageContext"></param>
	/// <param name="cancellationToken"></param>
	/// <returns></returns>
	Task HandleAsync(object message, MessageContext messageContext, CancellationToken cancellationToken = default);
}

/// <summary>
/// Contract of message handler.
/// </summary>
/// <typeparam name="TMessage">The type of the t message.</typeparam>
public interface IHandler<in TMessage> : IHandler
	where TMessage : class
{
	async Task IHandler.HandleAsync(object message, MessageContext messageContext, CancellationToken cancellationToken)
	{
		if (message is TMessage knownMessage)
		{
			await HandleAsync(knownMessage, messageContext, cancellationToken);
		}
	}

	/// <summary>
	/// Handle message.
	/// </summary>
	/// <param name="message">The message.</param>
	/// <param name="messageContext">The message context.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns>Task&lt;System.Boolean&gt;.</returns>
	Task HandleAsync(TMessage message, MessageContext messageContext, CancellationToken cancellationToken = default);
}