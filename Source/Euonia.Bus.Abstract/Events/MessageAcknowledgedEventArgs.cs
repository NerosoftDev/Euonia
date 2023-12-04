namespace Nerosoft.Euonia.Bus;

/// <summary>
/// Class MessageAcknowledgedEventArgs.
/// Implements the <see cref="MessageProcessedEventArgs" />
/// </summary>
/// <seealso cref="MessageProcessedEventArgs" />
public class MessageAcknowledgedEventArgs : MessageProcessedEventArgs
{
	/// <summary>
	/// Initializes a new instance of the <see cref="MessageAcknowledgedEventArgs"/> class.
	/// </summary>
	/// <param name="message">The message.</param>
	/// <param name="context">The message context.</param>
	public MessageAcknowledgedEventArgs(object message, IMessageContext context)
		: base(message, context, MessageProcessType.Receive)
	{
	}
}