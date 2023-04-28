using Nerosoft.Euonia.Domain;

namespace Nerosoft.Euonia.Bus;

/// <summary>
/// The abstract implement of <see cref="IMessageHandler" />.
/// Implements the <see cref="IMessageHandler" />
/// </summary>
/// <seealso cref="IMessageHandler" />
public abstract class MessageHandlerBase : IMessageHandler
{
    /// <summary>
    /// Determines whether the current message handler can handle the message with the specified message type.
    /// </summary>
    /// <param name="messageType">Type of the message to be checked.</param>
    /// <returns><c>true</c> if the current message handler can handle the message with the specified message type; otherwise, <c>false</c>.</returns>
    public abstract bool CanHandle(Type messageType);

    /// <summary>
    /// Handle message.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="messageContext">The message context.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>Task&lt;System.Boolean&gt;.</returns>
    public abstract Task HandleAsync(IMessage message, MessageContext messageContext, CancellationToken cancellationToken = default);
}

/// <summary>
/// The abstract implement of <see cref="IMessageHandler" />.
/// Implements the <see cref="MessageHandlerBase" />
/// Implements the <see cref="IMessageHandler{TMessage}" />
/// </summary>
/// <typeparam name="TMessage">The type of the message to be handled.</typeparam>
/// <seealso cref="MessageHandlerBase" />
/// <seealso cref="IMessageHandler{TMessage}" />
public abstract class MessageHandlerBase<TMessage> : MessageHandlerBase, IMessageHandler<TMessage>
    where TMessage : IMessage
{
    /// <summary>
    /// Determines whether this instance can handle the specified message type.
    /// </summary>
    /// <param name="messageType">Type of the message.</param>
    /// <returns><c>true</c> if this instance can handle the specified message type; otherwise, <c>false</c>.</returns>
    public override bool CanHandle(Type messageType)
    {
        return typeof(TMessage) == messageType;
    }

    /// <summary>
    /// handle as an asynchronous operation.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="messageContext">The message context.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <returns>Task&lt;System.Boolean&gt;.</returns>
    public sealed override Task HandleAsync(IMessage message, MessageContext messageContext, CancellationToken cancellationToken = default)
    {
        return message switch
        {
            null => throw new ArgumentNullException(nameof(message)),
            TMessage typedMessage => HandleAsync(typedMessage, messageContext, cancellationToken),
            _ => Task.CompletedTask
        };
    }

    /// <summary>
    /// Handles the asynchronous.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="messageContext">The message context.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>Task&lt;System.Boolean&gt;.</returns>
    public abstract Task HandleAsync(TMessage message, MessageContext messageContext, CancellationToken cancellationToken = default);
}