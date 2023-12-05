﻿using System.Net;

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

#if !NET8_0_OR_GREATER
	/// <inheritdoc />
	public UpgradeRequiredException(SerializationInfo info, StreamingContext context)
		: base(info, context)
	{
	}
#endif

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