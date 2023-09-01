using System.Net;
using System.Runtime.Serialization;

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

    /// <inheritdoc />
    protected ForbiddenException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }

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