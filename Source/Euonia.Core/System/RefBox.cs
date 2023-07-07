namespace System;

///<summary>
/// Represents a reference box that contains a value of a specified generic type.
/// This class enforces the following rules on its contents:
/// - The value contained will always be non-null
/// - Once created, the value contained will never change
/// - The value contained will never be boxed.
///</summary>
public sealed class RefBox<T> where T : struct
{
    private readonly T _value;

    internal RefBox(T value)
    {
        _value = value;
    }

    /// <summary>
    /// Gets the value.
    /// </summary>
    public ref readonly T Value => ref _value;
}

///<summary>
/// Represents a reference box that contains a value of a specified generic type.
/// This class enforces the following rules on its contents:
/// - The value contained will always be non-null
/// - Once created, the value contained will never change
/// - The value contained will never be boxed.
///</summary>
public sealed class RefBox
{
    /// <summary>
    /// Create a new instance of <see cref="RefBox{T}"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="value"></param>
    /// <returns></returns>
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