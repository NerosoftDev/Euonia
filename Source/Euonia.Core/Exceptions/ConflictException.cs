using System.Net;

namespace System;

/// <summary>
/// Represents errors that occur if conflict.
/// </summary>
[Serializable, HttpStatusCode(HttpStatusCode.Conflict)]
public class ConflictException : Exception
{
	private const string DEFAULT_MESSAGE = "Conflict";

	/// <inheritdoc />
	public ConflictException()
		: base(DEFAULT_MESSAGE)
	{
	}

	/// <inheritdoc />
	public ConflictException(string message)
		: base(message)
	{
	}

	/// <inheritdoc />
	public ConflictException(string message, Exception innerException)
		: base(message, innerException)
	{
	}

#pragma warning disable SYSLIB0051
	/// <inheritdoc />
	public ConflictException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}
#pragma warning restore SYSLIB0051
}