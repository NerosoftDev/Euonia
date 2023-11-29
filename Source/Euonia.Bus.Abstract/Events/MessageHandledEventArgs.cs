namespace Nerosoft.Euonia.Bus;

/// <summary>
/// Occurs when message was handled.
/// </summary>
public class MessageHandledEventArgs : EventArgs
{
	/// <summary>
	/// Initializes a new instance of the <see cref="MessageHandledEventArgs"/> class.
	/// </summary>
	/// <param name="message"></param>
	public MessageHandledEventArgs(object message)
	{
		Message = message;
	}

	/// <summary>
	/// Gets the handle message.
	/// </summary>
	public object Message { get; }

	/// <summary>
	/// Gets the handler type.
	/// </summary>
	public Type HandlerType { get; internal set; }
}