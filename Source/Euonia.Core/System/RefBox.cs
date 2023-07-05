namespace System;

public sealed class RefBox<T> where T : struct
{
    private readonly T _value;

    internal RefBox(T value)
    {
        _value = value;
    }

    public ref readonly T Value => ref _value;
}

public sealed class RefBox
{
    public static RefBox<T> Create<T>(T value) where T : struct => new(value);

    /// <summary>
    /// Thread-safely checks if <paramref name="boxRef"/> is non-null and if so sets it to null and outputs
    /// the value as <paramref name="value"/>.
    /// </summary>
    public static bool TryConsume<T>(ref RefBox<T> boxRef, out T value)
        where T : struct
    {
        var box = Interlocked.Exchange(ref boxRef, null);
        if (box != null)
        {
            value = box.Value;
            return true;
        }

        value = default;
        return false;
    }
}