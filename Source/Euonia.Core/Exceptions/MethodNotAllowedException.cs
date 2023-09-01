using System.Net;
using System.Runtime.Serialization;

namespace Nerosoft.Euonia.Core;

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

    /// <inheritdoc />
    protected MethodNotAllowedException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }

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