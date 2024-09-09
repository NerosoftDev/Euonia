namespace Nerosoft.Euonia.Bus;

/// <summary>
/// The message delivered event arguments
/// </summary>
public class MessageDeliveredEventArgs : MessageProcessedEventArgs
{
	/// <summary>
	/// 
	/// </summary>
	/// <param name="message"></param>
	/// <param name="context"></param>
	public MessageDeliveredEventArgs(object message, IMessageContext context)
		: base(message, context, MessageProcessType.Dispatch)
	{
	}
}