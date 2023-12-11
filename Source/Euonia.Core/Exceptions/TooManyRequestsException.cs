using System.Net;

namespace System;

/// <summary>
/// Represents errors that occur if too many requests.
/// </summary>
[Serializable, HttpStatusCode(HttpStatusCode.TooManyRequests)]
public class TooManyRequestsException : Exception
{
	private const string DEFAULT_MESSAGE = "Too Many Requests";

	/// <inheritdoc />
	public TooManyRequestsException()
		: base(DEFAULT_MESSAGE)
	{
	}

	/// <inheritdoc />
	public TooManyRequestsException(string message)
		: base(message)
	{
	}

	/// <inheritdoc />
	public TooManyRequestsException(string message, Exception innerException)
		: base(message, innerException)
	{
	}

#pragma warning disable SYSLIB0051
	/// <inheritdoc />
	public TooManyRequestsException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}
#pragma warning restore SYSLIB0051
}