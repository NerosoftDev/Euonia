using Nerosoft.Euonia.Domain;

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
    /// <param name="messageContext">The message context.</param>
    /// <param name="processType">Type of the process.</param>
    public MessageProcessedEventArgs(IMessage message, MessageContext messageContext, MessageProcessType processType)
    {
        Message = message;
        MessageContext = messageContext;
        ProcessType = processType;
    }

    /// <summary>
    /// Gets the message.
    /// </summary>
    /// <value>The message.</value>
    public IMessage Message { get; }

    /// <summary>
    /// Gets the message context.
    /// </summary>
    /// <value>The message context.</value>
    public MessageContext MessageContext { get; }

    /// <summary>
    /// Gets the type of the process.
    /// </summary>
    /// <value>The type of the process.</value>
    public MessageProcessType ProcessType { get; }
}