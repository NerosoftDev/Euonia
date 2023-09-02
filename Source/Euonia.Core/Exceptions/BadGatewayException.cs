using System.Net;
using System.Runtime.Serialization;

namespace Nerosoft.Euonia.Core;

/// <summary>
/// Represents errors that occur when a server acting as a gateway or proxy received an invalid response
/// from an inbound server it accessed while attempting to fulfill the request.
/// </summary>
[Serializable, HttpStatusCode(HttpStatusCode.BadGateway)]
public class BadGatewayException : Exception
{
    private const string DEFAULT_MESSAGE = "Bad Gateway";

    /// <inheritdoc />
    public BadGatewayException()
        : base(DEFAULT_MESSAGE)
    {
    }

    /// <inheritdoc />
    protected BadGatewayException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }

    /// <inheritdoc />
    public BadGatewayException(string message)
        : base(message)
    {
    }

    /// <inheritdoc />
    public BadGatewayException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}