using System.Net;
using System.Runtime.Serialization;

namespace Nerosoft.Euonia.Core;

/// <summary>
/// Represents errors that occur if upgrade required.
/// </summary>
[Serializable, HttpStatusCode(HttpStatusCode.UpgradeRequired)]
public class UpgradeRequiredException : Exception
{
    private const string DEFAULT_MESSAGE = "Upgrade Required";

    /// <inheritdoc />
    public UpgradeRequiredException()
        : base(DEFAULT_MESSAGE)
    {
    }

    /// <inheritdoc />
    protected UpgradeRequiredException(SerializationInfo info, StreamingContext context)
        : base(info, context)
    {
    }

    /// <inheritdoc />
    public UpgradeRequiredException(string message)
        : base(message)
    {
    }

    /// <inheritdoc />
    public UpgradeRequiredException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}