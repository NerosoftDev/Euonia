namespace Nerosoft.Euonia.Bus;

/// <summary>
/// Contract of message handler.
/// </summary>
public interface IHandler
{
	/*
	/// <summary>
	/// Determines whether the current message handler can handle the message with the specified message type.
	/// </summary>
	/// <param name="messageType">Type of the message to be checked.</param>
	/// <returns><c>true</c> if the current message handler can handle the message with the specified message type; otherwise, <c>false</c>.</returns>
	bool CanHandle(Type messageType);
	*/
}

/// <summary>
/// Defines a contract for handling messages of a specific type and returning a response.
/// </summary>
/// <typeparam name="TMessage"></typeparam>
/// <typeparam name="TResponse"></typeparam>
public interface IHandler<in TMessage, TResponse> : IHandler
{
	/// <summary>
	/// Handle message.
	/// </summary>
	/// <param name="message">The message.</param>
	/// <param name="context">The message context.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns></returns>
	Task<TResponse> HandleAsync(TMessage message, MessageContext context, CancellationToken cancellationToken = default);
}

/// <summary>
/// Contract of message handler.
/// </summary>
/// <typeparam name="TMessage">The type of the t message.</typeparam>
public interface IHandler<in TMessage> : IHandler<TMessage, Unit>
	where TMessage : class
{
	/// <summary>
	/// Handle message.
	/// </summary>
	/// <param name="message">The message.</param>
	/// <param name="context">The message context.</param>
	/// <param name="cancellationToken">The cancellation token.</param>
	/// <returns></returns>
	new Task HandleAsync(TMessage message, MessageContext context, CancellationToken cancellationToken = default);

	Task<Unit> IHandler<TMessage, Unit>.HandleAsync(TMessage message, MessageContext context, CancellationToken cancellationToken)
	{
		return HandleAsync(message, context, cancellationToken).ContinueWith(_ => Unit.Value, cancellationToken);
	}
}