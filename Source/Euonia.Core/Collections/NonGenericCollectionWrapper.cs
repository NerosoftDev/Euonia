namespace Nerosoft.Euonia.Collections;

internal sealed class NonGenericCollectionWrapper<T> : IReadOnlyCollection<T>
{
    private readonly ICollection _collection;

    public NonGenericCollectionWrapper(ICollection collection)
    {
        _collection = collection ?? throw new ArgumentNullException(nameof(collection));
    }

    public int Count => _collection.Count;

    public IEnumerator<T> GetEnumerator()
    {
        foreach (T item in _collection)
        {
            yield return item;
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return _collection.GetEnumerator();
    }
}

internal sealed class CollectionWrapper<T> : IReadOnlyCollection<T>
{
    private readonly ICollection<T> _collection;

    public CollectionWrapper(ICollection<T> collection)
    {
        _collection = collection ?? throw new ArgumentNullException(nameof(collection));
    }

    public int Count => _collection.Count;

    public IEnumerator<T> GetEnumerator()
    {
        return _collection.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return _collection.GetEnumerator();
    }
}
