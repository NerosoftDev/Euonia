using System.Net;

namespace System;

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

#if !NET8_0_OR_GREATER
	/// <inheritdoc />
	public BadGatewayException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}
#endif

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