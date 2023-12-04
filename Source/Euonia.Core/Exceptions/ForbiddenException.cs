using System.Net;

namespace Nerosoft.Euonia.Core;

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

#if !NET8_0_OR_GREATER
	/// <inheritdoc />
	public ForbiddenException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}
#endif

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
}