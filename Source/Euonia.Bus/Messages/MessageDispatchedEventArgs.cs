using Nerosoft.Euonia.Domain;

namespace Nerosoft.Euonia.Bus;

/// <summary>
/// Class MessageDispatchedEventArgs.
/// Implements the <see cref="MessageProcessedEventArgs" />
/// </summary>
/// <seealso cref="MessageProcessedEventArgs" />
public class MessageDispatchedEventArgs : MessageProcessedEventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MessageDispatchedEventArgs"/> class.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="messageContext">The message context.</param>
    public MessageDispatchedEventArgs(IMessage message, MessageContext messageContext)
        : base(message, messageContext, MessageProcessType.Dispatch)
    {
    }
}