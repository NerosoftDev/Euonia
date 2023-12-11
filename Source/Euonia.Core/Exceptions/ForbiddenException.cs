using System.Net;

namespace System;

/// <summary>
/// Represents errors that occur if request is denied.
/// </summary>
[Serializable, HttpStatusCode(HttpStatusCode.Forbidden)]
public class ForbiddenException : Exception
{
	private const string DEFAULT_MESSAGE = "Forbidden";

	/// <inheritdoc />
	public ForbiddenException()
		: base(DEFAULT_MESSAGE)
	{
	}

	/// <inheritdoc />
	public ForbiddenException(string message)
		: base(message)
	{
	}

	/// <inheritdoc />
	public ForbiddenException(string message, Exception innerException)
		: base(message, innerException)
	{
	}

#pragma warning disable SYSLIB0051
	/// <inheritdoc />
	public ForbiddenException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}
#pragma warning restore SYSLIB0051
}