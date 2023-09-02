using System.Net;
using System.Runtime.Serialization;

namespace Nerosoft.Euonia.Core;

/// <summary>
/// Represents errors that occur if gateway timeout.
/// </summary>
[Serializable, HttpStatusCode(HttpStatusCode.GatewayTimeout)]
public class GatewayTimeoutException : Exception
{
    private const string DEFAULT_MESSAGE = "Gateway Timeout";

    /// <inheritdoc />
    public GatewayTimeoutException()
        : base(DEFAULT_MESSAGE)
    {
    }

    /// <inheritdoc />
    protected GatewayTimeoutException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }

    /// <inheritdoc />
    public GatewayTimeoutException(string message)
        : base(message)
    {
    }

    /// <inheritdoc />
    public GatewayTimeoutException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}