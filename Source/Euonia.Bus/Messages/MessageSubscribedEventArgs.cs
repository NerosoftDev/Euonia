namespace Nerosoft.Euonia.Bus;

/// <summary>
/// Class MessageSubscribedEventArgs.
/// Implements the <see cref="EventArgs" />
/// </summary>
/// <seealso cref="EventArgs" />
public class MessageSubscribedEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MessageSubscribedEventArgs"/> class.
    /// </summary>
    /// <param name="messageType">Type of the message.</param>
    /// <param name="handlerType">Type of the handler.</param>
    public MessageSubscribedEventArgs(Type messageType, Type handlerType)
        : this(messageType.FullName, handlerType)
    {
        MessageType = messageType;
    }

    public MessageSubscribedEventArgs(string messageName, Type handlerType)
    {
        MessageName = messageName;
        HandlerType = handlerType;
    }

    /// <summary>
    /// Gets the type of the message.
    /// </summary>
    /// <value>The type of the message.</value>
    public Type MessageType { get; }

    /// <summary>
    /// Gets the message name.
    /// </summary>
    public string MessageName { get; }

    /// <summary>
    /// Gets the type of the handler.
    /// </summary>
    /// <value>The type of the handler.</value>
    public Type HandlerType { get; }
}