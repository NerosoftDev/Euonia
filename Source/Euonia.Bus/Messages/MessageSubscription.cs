namespace Nerosoft.Euonia.Bus;

public class MessageSubscription
{
    public MessageSubscription(Type messageType, Type handlerType)
        : this(messageType.FullName, handlerType)
    {
        MessageType = messageType;
    }

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