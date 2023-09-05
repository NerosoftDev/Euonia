namespace Nerosoft.Euonia.Bus;

/// <summary>
/// The message subscription.
/// </summary>
public class MessageSubscription
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MessageSubscription"/> class.
    /// </summary>
    /// <param name="messageType"></param>
    /// <param name="handlerType"></param>
    public MessageSubscription(Type messageType, Type handlerType)
        : this(messageType.FullName, handlerType)
    {
        MessageType = messageType;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MessageSubscription"/> class.
    /// </summary>
    /// <param name="messageName"></param>
    /// <param name="handlerType"></param>
    public MessageSubscription(string messageName, Type handlerType)
    {
        MessageName = messageName;
        HandlerType = handlerType;
    }

    /// <summary>
    /// Gets or sets the message name.
    /// </summary>
    public string MessageName { get; set; }

    /// <summary>
    /// Gets or sets the message type.
    /// </summary>
    public Type MessageType { get; set; }

    /// <summary>
    /// Gets or sets the message handler type.
    /// </summary>
    public Type HandlerType { get; set; }
}