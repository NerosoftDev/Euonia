using System.Net;

namespace System;

/// <summary>
/// Represents the exception that is thrown when an internal server error occurs.
/// </summary>
[Serializable, HttpStatusCode(HttpStatusCode.InternalServerError)]
public class InternalServerErrorException : Exception
{
    private const string DEFAULT_MESSAGE = "Internal Server Error";

    /// <inheritdoc />
    public InternalServerErrorException()
        : base(DEFAULT_MESSAGE)
    {
    }

#if !NET8_0_OR_GREATER
	/// <inheritdoc />
	public InternalServerErrorException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	} 
#endif

	/// <inheritdoc />
	public InternalServerErrorException(string message)
        : base(message)
    {
    }

    /// <inheritdoc />
    public InternalServerErrorException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}