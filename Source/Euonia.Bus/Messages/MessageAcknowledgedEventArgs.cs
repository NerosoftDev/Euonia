using Nerosoft.Euonia.Domain;

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
    /// <param name="messageContext">The message context.</param>
    public MessageAcknowledgedEventArgs(IMessage message, MessageContext messageContext)
        : base(message, messageContext, MessageProcessType.Receive)
    {
    }
}