namespace Nerosoft.Euonia.Bus;

/// <summary>
/// Class MessageReceivedEventArgs.
/// Implements the <see cref="MessageProcessedEventArgs" />
/// </summary>
/// <seealso cref="MessageProcessedEventArgs" />
public class MessageReceivedEventArgs : MessageProcessedEventArgs
{
	/// <summary>
	/// Initializes a new instance of the <see cref="MessageReceivedEventArgs"/> class.
	/// </summary>
	/// <param name="message">The message.</param>
	/// <param name="context">The message context.</param>
	public MessageReceivedEventArgs(object message, IMessageContext context)
		: base(message, context, MessageProcessType.Receive)
	{
	}
}