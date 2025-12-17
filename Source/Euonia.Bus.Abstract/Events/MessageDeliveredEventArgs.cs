namespace Nerosoft.Euonia.Bus;

/// <summary>
/// Class MessageDispatchedEventArgs.
/// Implements the <see cref="MessageProcessedEventArgs" />
/// </summary>
/// <seealso cref="MessageProcessedEventArgs" />
public class MessageDeliveredEventArgs : MessageProcessedEventArgs
{
	/// <summary>
	/// Initializes a new instance of the <see cref="MessageDeliveredEventArgs"/> class.
	/// </summary>
	/// <param name="message">The message.</param>
	/// <param name="context">The message context.</param>
	public MessageDeliveredEventArgs(object message, IMessageContext context)
		: base(message, context, MessageProcessType.Dispatch)
	{
	}
}