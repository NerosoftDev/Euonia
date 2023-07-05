using System.Diagnostics;

namespace System;

public static class Invariant
{
    [Conditional("DEBUG")]
    public static void Require(bool condition, string message = null)
    {
        if (!condition)
        {
            throw new InvalidOperationException(message ?? "invariant violated");
        }
    }
}