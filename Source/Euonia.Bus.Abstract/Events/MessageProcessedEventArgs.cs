namespace Nerosoft.Euonia.Bus;

/// <summary>
/// Class MessageProcessedEventArgs.
/// Implements the <see cref="EventArgs" />
/// </summary>
/// <seealso cref="EventArgs" />
public class MessageProcessedEventArgs : EventArgs
{
	/// <summary>
	/// Initializes a new instance of the <see cref="MessageProcessedEventArgs"/> class.
	/// </summary>
	/// <param name="message">The message.</param>
	/// <param name="context">The message context.</param>
	/// <param name="processType">Type of the process.</param>
	public MessageProcessedEventArgs(object message, IMessageContext context, MessageProcessType processType)
	{
		Message = message;
		Context = context;
		ProcessType = processType;
	}

	/// <summary>
	/// Gets the message.
	/// </summary>
	/// <value>The message.</value>
	public object Message { get; }

	/// <summary>
	/// Gets the message id.
	/// </summary>
	/// <value>The message id.</value>
	public IMessageContext Context { get; }

	/// <summary>
	/// Gets the type of the process.
	/// </summary>
	/// <value>The type of the process.</value>
	public MessageProcessType ProcessType { get; }
}