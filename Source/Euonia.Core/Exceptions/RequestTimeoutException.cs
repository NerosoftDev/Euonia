using System.Net;
using System.Runtime.Serialization;

namespace Nerosoft.Euonia.Core;

/// <summary>
/// Represents errors that occur if request timeout.
/// </summary>
[Serializable, HttpStatusCode(HttpStatusCode.RequestTimeout)]
public class RequestTimeoutException : Exception
{
    private const string DEFAULT_MESSAGE = "Request Timeout";

    /// <inheritdoc />
    public RequestTimeoutException()
        : base(DEFAULT_MESSAGE)
    {
    }

    /// <inheritdoc />
    protected RequestTimeoutException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }

    /// <inheritdoc />
    public RequestTimeoutException(string message)
        : base(message)
    {
    }

    /// <inheritdoc />
    public RequestTimeoutException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}