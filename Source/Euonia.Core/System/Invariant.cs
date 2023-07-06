using System.Diagnostics;

namespace System;

/// <summary>
/// Methods help to enforce invariants.
/// </summary>
public static class Invariant
{
    /// <summary>
    /// Method that checks if a condition is true, and if not, throws an InvalidOperationException with a specified message.
    /// </summary>
    /// <param name="condition">The condition to check.</param>
    /// <param name="message">The message to include in the InvalidOperationException if the condition is false. If null, a default message is used.</param>
    [Conditional("DEBUG")]
    public static void Require(bool condition, string message = null)
    {
        if (!condition)
        {
            throw new InvalidOperationException(message ?? "invariant violated");
        }
    }
}