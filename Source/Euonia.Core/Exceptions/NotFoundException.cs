using System.Net;
using System.Runtime.Serialization;

namespace Nerosoft.Euonia.Core;

/// <summary>
/// Represents errors that occur if data is not found.
/// </summary>
[Serializable, HttpStatusCode(HttpStatusCode.NotFound)]
public class NotFoundException : Exception
{
    private const string DEFAULT_MESSAGE = "Not Found";

    /// <inheritdoc />
    public NotFoundException()
        : base(DEFAULT_MESSAGE)
    {
    }

    /// <inheritdoc />
    public NotFoundException(string message)
        : base(message)
    {
    }

    /// <inheritdoc />
    public NotFoundException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    /// <inheritdoc />
    protected NotFoundException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }
}