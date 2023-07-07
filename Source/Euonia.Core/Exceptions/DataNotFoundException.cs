using System.Data;
using System.Runtime.Serialization;

namespace Nerosoft.Euonia.Core;

/// <summary>
/// Represents errors that occur if data is not found.
/// </summary>
[Serializable]
public class DataNotFoundException : DataException
{
    private const string DEFAULT_MESSAGE = "Data not found";

    /// <summary>
    /// Initializes a new instance of the <see cref="DataNotFoundException"/> class.
    /// </summary>
    public DataNotFoundException()
        : base(DEFAULT_MESSAGE)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DataNotFoundException"/> class with a specified error message.
    /// </summary>
    /// <param name="message"></param>
    public DataNotFoundException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DataNotFoundException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message"></param>
    /// <param name="innerException"></param>
    public DataNotFoundException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    /// <inheritdoc />
    protected DataNotFoundException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }
}
