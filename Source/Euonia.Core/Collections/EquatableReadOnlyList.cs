namespace Nerosoft.Euonia.Collections;

/// <summary>
/// Represents a read-only list that enabled comparison of two instance of the <see cref="EquatableReadOnlyList{T}"/> for equality.
/// </summary>
/// <typeparam name="T">The element type.</typeparam>
public readonly struct EquatableReadOnlyList<T> : IReadOnlyList<T>, IEquatable<EquatableReadOnlyList<T>>
{
    private readonly T[] _array;

    /// <summary>
    /// Initializes a new instance of the <see cref="EquatableReadOnlyList{T}"/> class.
    /// </summary>
    /// <param name="items"></param>
    public EquatableReadOnlyList(IEnumerable<T> items)
    {
        _array = items.ToArray();
    }

    /// <inheritdoc />
    public T this[int index] => _array[index];

    /// <summary>
    /// Gets the element count of the list.
    /// </summary>
    public int Count => _array.Length;

    /// <inheritdoc />
    public bool Equals(EquatableReadOnlyList<T> other) => _array.SequenceEqual(other._array);

    /// <inheritdoc />
    public override bool Equals(object obj) => obj is EquatableReadOnlyList<T> that && Equals(that);

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return _array.Aggregate(0, (current, item) => (current, item).GetHashCode());
    }

    /// <summary>
    /// Returns an enumerator that iterates through the collection.
    /// </summary>
    /// <returns></returns>
    public IEnumerator<T> GetEnumerator() => _array.As<IEnumerable<T>>().GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <inheritdoc />
    public static bool operator ==(EquatableReadOnlyList<T> left, EquatableReadOnlyList<T> right)
    {
        return left.Equals(right);
    }

    /// <inheritdoc />
    public static bool operator !=(EquatableReadOnlyList<T> left, EquatableReadOnlyList<T> right)
    {
        return !(left == right);
    }
}