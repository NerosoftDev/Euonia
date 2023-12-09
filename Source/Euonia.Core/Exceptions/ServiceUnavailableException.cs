using System.Net;

namespace System;

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

#if !NET8_0_OR_GREATER
	/// <inheritdoc />
	public ServiceUnavailableException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	} 
#endif

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