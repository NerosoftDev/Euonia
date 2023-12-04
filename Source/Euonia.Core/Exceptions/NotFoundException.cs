using System.Net;

namespace Nerosoft.Euonia.Core;

/// <summary>
/// Represents errors that occur if data is not found.
/// </summary>
[Serializable, HttpStatusCode(HttpStatusCode.NotFound)]
public class NotFoundException : Exception
{
    private const string DEFAULT_MESSAGE = "Not Found";

    /// <inheritdoc />
    public NotFoundException()
        : base(DEFAULT_MESSAGE)
    {
    }

    /// <inheritdoc />
    public NotFoundException(string message)
        : base(message)
    {
    }

    /// <inheritdoc />
    public NotFoundException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

#if !NET8_0_OR_GREATER
	/// <inheritdoc />
	public NotFoundException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	} 
#endif
}