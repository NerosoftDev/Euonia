using System.Net;

namespace Nerosoft.Euonia.Core;

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

#if !NET8_0_OR_GREATER
	/// <inheritdoc />
	public RequestTimeoutException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}
#endif

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
}