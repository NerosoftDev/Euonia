using System.Net;

namespace Nerosoft.Euonia.Core;

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

#if !NET8_0_OR_GREATER
	/// <inheritdoc />
	public TooManyRequestsException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}
#endif

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
}