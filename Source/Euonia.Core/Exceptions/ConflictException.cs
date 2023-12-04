using System.Net;

namespace Nerosoft.Euonia.Core;

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

#if !NET8_0_OR_GREATER
	/// <inheritdoc />
	public ConflictException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}
#endif

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
}