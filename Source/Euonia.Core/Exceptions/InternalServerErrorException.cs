using System.Net;
using System.Runtime.Serialization;

namespace Nerosoft.Euonia.Core;

/// <summary>
/// Represents the exception that is thrown when an internal server error occurs.
/// </summary>
[Serializable, HttpStatusCode(HttpStatusCode.InternalServerError)]
public class InternalServerErrorException : Exception
{
    private const string DEFAULT_MESSAGE = "Internal Server Error";

    /// <inheritdoc />
    public InternalServerErrorException()
        : base(DEFAULT_MESSAGE)
    {
    }

    /// <inheritdoc />
    protected InternalServerErrorException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }

    /// <inheritdoc />
    public InternalServerErrorException(string message)
        : base(message)
    {
    }

    /// <inheritdoc />
    public InternalServerErrorException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}