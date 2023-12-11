using System.Net;

namespace System;

/// <summary>
/// Represents errors that occur if request is bad.
/// </summary>
[Serializable, HttpStatusCode(HttpStatusCode.BadRequest)]
public class BadRequestException : Exception
{
    private const string DEFAULT_MESSAGE = "Bad Request";

    /// <inheritdoc />
    public BadRequestException()
        : base(DEFAULT_MESSAGE)
    {
    }

    /// <inheritdoc />
    public BadRequestException(string message)
        : base(message)
    {
    }

    /// <inheritdoc />
    public BadRequestException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
    
#pragma warning disable SYSLIB0051
    /// <inheritdoc />
    public BadRequestException(SerializationInfo info, StreamingContext context)
	    : base(info, context)
    {
    }
#pragma warning restore SYSLIB0051
}