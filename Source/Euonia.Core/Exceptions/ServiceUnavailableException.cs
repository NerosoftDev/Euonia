using System.Net;
using System.Runtime.Serialization;

namespace Nerosoft.Euonia.Core;

/// <summary>
/// Represents errors that occur if service is unavailable.
/// </summary>
[Serializable, HttpStatusCode(HttpStatusCode.ServiceUnavailable)]
public class ServiceUnavailableException : Exception
{
    private const string DEFAULT_MESSAGE = "Service Unavailable";

    /// <inheritdoc />
    public ServiceUnavailableException()
        : base(DEFAULT_MESSAGE)
    {
    }

    /// <inheritdoc />
    protected ServiceUnavailableException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }

    /// <inheritdoc />
    public ServiceUnavailableException(string message)
        : base(message)
    {
    }

    /// <inheritdoc />
    public ServiceUnavailableException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}