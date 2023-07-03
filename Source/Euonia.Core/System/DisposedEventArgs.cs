namespace System;

/// <inheritdoc />
public class DisposedEventArgs : EventArgs
{
    /// <summary>
    /// Initialize a new instance of <see cref="DisposedEventArgs"/>.
    /// </summary>
    public DisposedEventArgs()
    {
    }

    /// <summary>
    /// Initialize a new instance of <see cref="DisposedEventArgs"/>.
    /// </summary>
    /// <param name="hashCode">The hash code of the disposed object.</param>
    public DisposedEventArgs(int hashCode)
        : this()
    {
        HashCode = hashCode;
    }

    /// <summary>
    /// Gets the hash code of the disposed object.
    /// </summary>
    public int HashCode { get; }
}