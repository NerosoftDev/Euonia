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

#if !NET8_0_OR_GREATER
	/// <inheritdoc />
	public MethodNotAllowedException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	} 
#endif

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
}