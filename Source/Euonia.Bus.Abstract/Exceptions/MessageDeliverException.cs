namespace Nerosoft.Euonia.Bus;

/// <summary>
/// Represents errors that occur during message deliver.
/// </summary>
public class MessageDeliverException : Exception
{
	/// <summary>
	/// Initializes a new instance of the <see cref="MessageDeliverException"/> class.
	/// </summary>
	public MessageDeliverException()
		: base("Error occurred during message deliver.")
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="MessageDeliverException"/> class.
	/// </summary>
	/// <param name="message"></param>
	public MessageDeliverException(string message)
		: base(message)
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="MessageDeliverException"/> class.
	/// </summary>
	/// <param name="message"></param>
	/// <param name="innerException"></param>
	public MessageDeliverException(string message, Exception innerException)
		: base(message, innerException)
	{
	}
}
