using System.Net;

namespace System;

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
    public UpgradeRequiredException(string message)
        : base(message)
    {
    }

    /// <inheritdoc />
    public UpgradeRequiredException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
    
#pragma warning disable SYSLIB0051
    /// <inheritdoc />
    public UpgradeRequiredException(SerializationInfo info, StreamingContext context)
	    : base(info, context)
    {
    }
#pragma warning restore SYSLIB0051
}