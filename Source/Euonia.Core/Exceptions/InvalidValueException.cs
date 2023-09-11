using System.Runtime.Serialization;

namespace Nerosoft.Euonia.Core;

/// <summary>
/// Represents the errors occurring when a value is invalid.
/// </summary>
[Serializable]
public class InvalidValueException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidValueException"/> class.
    /// </summary>
    public InvalidValueException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidValueException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The error message.</param>
    public InvalidValueException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidValueException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="innerException">The inner exception that is the cause of this exception.</param>
    public InvalidValueException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidValueException"/> class with serialized data.
    /// </summary>
    /// <param name="info">The exception data serialize information.</param>
    /// <param name="context"></param>
    protected InvalidValueException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }
}
