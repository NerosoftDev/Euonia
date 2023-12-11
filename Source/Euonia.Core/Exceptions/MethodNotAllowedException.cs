using System.Net;

namespace System;

/// <summary>
/// Represents errors that occur if request method is not allowed.
/// </summary>
[Serializable, HttpStatusCode(HttpStatusCode.MethodNotAllowed)]
public class MethodNotAllowedException : Exception
{
	private const string DEFAULT_MESSAGE = "Method Not Allowed";

	/// <inheritdoc />
	public MethodNotAllowedException()
		: base(DEFAULT_MESSAGE)
	{
	}

	/// <inheritdoc />
	public MethodNotAllowedException(string message)
		: base(message)
	{
	}

	/// <inheritdoc />
	public MethodNotAllowedException(string message, Exception innerException)
		: base(message, innerException)
	{
	}

#pragma warning disable SYSLIB0051
	/// <inheritdoc />
	public MethodNotAllowedException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}
#pragma warning restore SYSLIB0051
}