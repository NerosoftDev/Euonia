using System.Net;

namespace System;

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
	public RequestTimeoutException(string message)
		: base(message)
	{
	}

	/// <inheritdoc />
	public RequestTimeoutException(string message, Exception innerException)
		: base(message, innerException)
	{
	}

#pragma warning disable SYSLIB0051
	/// <inheritdoc />
	public RequestTimeoutException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}
#pragma warning restore SYSLIB0051
}