namespace Nerosoft.Euonia.Bus;

/// <summary>
/// Represents errors that occur when the message type is invalid.
/// </summary>
public class MessageTypeException : Exception
{
	/// <summary>
	/// Initializes a new instance of the <see cref="MessageTypeException"/> class.
	/// </summary>
	public MessageTypeException()
		: base("The message type is invalid.")
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="MessageTypeException"/> class.
	/// </summary>
	/// <param name="message"></param>
	public MessageTypeException(string message)
		: base(message)
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="MessageTypeException"/> class.
	/// </summary>
	/// <param name="message"></param>
	/// <param name="innerException"></param>
	public MessageTypeException(string message, Exception innerException)
		: base(message, innerException)
	{
	}
}
