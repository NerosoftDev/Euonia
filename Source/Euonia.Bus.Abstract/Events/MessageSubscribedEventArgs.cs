namespace Nerosoft.Euonia.Bus;

/// <summary>
/// Represents the message was subscribed.
/// </summary>
public class MessageSubscribedEventArgs : EventArgs
{
	/// <summary>
	/// Initializes a new instance of the <see cref="MessageSubscribedEventArgs"/> class.
	/// </summary>
	/// <param name="channel"></param>
	/// <param name="messageType"></param>
	/// <param name="handlerType"></param>
	public MessageSubscribedEventArgs(string channel, Type messageType, Type handlerType)
	{
		Channel = channel;
		MessageType = messageType;
		HandlerType = handlerType;
	}

	/// <summary>
	/// Gets the message name.
	/// </summary>
	public string Channel { get; }

	/// <summary>
	/// Gets the type of the message.
	/// </summary>
	/// <value>The type of the message.</value>
	public Type MessageType { get; }

	/// <summary>
	/// Gets the type of the handler.
	/// </summary>
	/// <value>The type of the handler.</value>
	public Type HandlerType { get; }
}