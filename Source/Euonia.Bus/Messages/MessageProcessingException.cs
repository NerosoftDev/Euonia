namespace Nerosoft.Euonia.Bus;

/// <summary>
/// Class MessageProcessingException.
/// Implements the <see cref="Exception" />
/// </summary>
/// <seealso cref="Exception" />
public class MessageProcessingException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MessageProcessingException" /> class.
    /// </summary>
    public MessageProcessingException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MessageProcessingException" /> class.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public MessageProcessingException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MessageProcessingException" /> class.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified.</param>
    public MessageProcessingException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}