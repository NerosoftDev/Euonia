namespace Nerosoft.Euonia.Domain;

/// <summary>
/// The base contract of message.
/// </summary>
public interface IMessage
{
    /// <summary>
    /// Gets or sets the message identifier.
    /// </summary>
    Guid Id { get; }

    /// <summary>
    /// Gets or sets the timestamp.
    /// </summary>
    DateTime Timestamp { get; }

    /// <summary>
    /// Gets the message meta data.
    /// </summary>
    MessageMetadata Metadata { get; }

    /// <summary>
    /// Gets the assembly qualified name of the current message.
    /// </summary>
    /// <returns>The type of current message.</returns>
    string GetTypeName();
}