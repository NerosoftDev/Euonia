using MediatR;
using Nerosoft.Euonia.Domain;

namespace Nerosoft.Euonia.Bus;

/// <summary>
/// Specifies contract of message handler context.
/// </summary>
public interface IMessageHandlerContext
{
    event EventHandler<MessageSubscribedEventArgs> MessageSubscribed;

    /// <summary>
    /// Registers handler of <typeparamref name="THandler" /> for <typeparamref name="TMessage" />.
    /// </summary>
    /// <typeparam name="TMessage">The type of the message.</typeparam>
    /// <typeparam name="THandler">The type of the handler.</typeparam>
    void Register<TMessage, THandler>()
        where TMessage : IMessage
        where THandler : IMessageHandler<TMessage>;

    /// <summary>
    /// Registers handler for message of type <paramref name="messageType" />
    /// </summary>
    /// <param name="messageType">Type of the message.</param>
    /// <param name="handlerType">Type of the handler.</param>
    void Register(Type messageType, Type handlerType);

    /// <summary>
    /// Register handler for named message.
    /// </summary>
    /// <param name="messageName"></param>
    /// <param name="handlerType"></param>
    void Register(string messageName, Type handlerType);

    /// <summary>
    /// Determine whether the message would be handled by a handler of type <typeparamref name="THandler" />.
    /// </summary>
    /// <typeparam name="TMessage">The type of the message.</typeparam>
    /// <typeparam name="THandler">The type of the handler.</typeparam>
    /// <returns><c>true</c> if [is handler registered]; otherwise, <c>false</c>.</returns>
    bool IsHandlerRegistered<TMessage, THandler>()
        where TMessage : IMessage
        where THandler : IMessageHandler<TMessage>;

    /// <summary>
    /// Determine whether the message would be handled by a handler of type <paramref name="handlerType" />.
    /// </summary>
    /// <param name="messageType">The type of the message.</param>
    /// <param name="handlerType">The type of the handler.</param>
    /// <returns><c>true</c> if [is handler registered] [the specified message type]; otherwise, <c>false</c>.</returns>
    bool IsHandlerRegistered(Type messageType, Type handlerType);

    /// <summary>
    /// Determine whether the message would be handled by a handle of type <paramref name="handlerType"/>.
    /// </summary>
    /// <param name="messageName">The message name.</param>
    /// <param name="handlerType">The type of the handler.</param>
    /// <returns><c>true</c> if [is handler registered] [the specified message type]; otherwise, <c>false</c>.</returns>
    bool IsHandlerRegistered(string messageName, Type handlerType);

    /// <summary>
    /// Handle message asynchronously.
    /// </summary>
    /// <param name="message">The message to be handled.</param>
    /// <param name="context">The message context.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>Task.</returns>
    Task HandleAsync(IMessage message, MessageContext context, CancellationToken cancellationToken = default);

    IMediator GetMediator();
}