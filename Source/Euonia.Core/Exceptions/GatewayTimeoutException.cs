using System.Net;

namespace System;

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

#if !NET8_0_OR_GREATER
	/// <inheritdoc />
	public GatewayTimeoutException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	} 
#endif

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