using System.Net;
using System.Runtime.Serialization;

namespace Nerosoft.Euonia.Core;

/// <summary>
/// Represents errors that occur if request is bad.
/// </summary>
[Serializable, HttpStatusCode(HttpStatusCode.BadRequest)]
public class BadRequestException : Exception
{
    private const string DEFAULT_MESSAGE = "Bad Request";

    /// <inheritdoc />
    public BadRequestException()
        : base(DEFAULT_MESSAGE)
    {
    }

    /// <inheritdoc />
    protected BadRequestException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }

    /// <inheritdoc />
    public BadRequestException(string message)
        : base(message)
    {
    }

    /// <inheritdoc />
    public BadRequestException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}