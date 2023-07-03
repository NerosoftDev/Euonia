using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;

namespace Nerosoft.Euonia.Collections;

/// <summary>
/// An observable list of observable groups.
/// </summary>
/// <typeparam name="TKey">The type of the group key.</typeparam>
/// <typeparam name="TValue">The type of the items in the collection.</typeparam>
public sealed class ObservableGroupedCollection<TKey, TValue> : ObservableCollection<ObservableGroup<TKey, TValue>>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ObservableGroupedCollection{TKey, TValue}"/> class.
    /// </summary>
    public ObservableGroupedCollection()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ObservableGroupedCollection{TKey, TValue}"/> class.
    /// </summary>
    /// <param name="collection">The initial data to add in the grouped collection.</param>
    public ObservableGroupedCollection(IEnumerable<IGrouping<TKey, TValue>> collection)
        : base(collection.Select(c => new ObservableGroup<TKey, TValue>(c)))
    {
    }

    /// <summary>
    /// Tries to get the underlying <see cref="List{T}"/> instance, if present.
    /// </summary>
    /// <param name="list">The resulting <see cref="List{T}"/>, if one was in use.</param>
    /// <returns>Whether or not a <see cref="List{T}"/> instance has been found.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal bool TryGetList(out List<ObservableGroup<TKey, TValue>> list)
    {
        list = Items as List<ObservableGroup<TKey, TValue>>;

        return list is not null;
    }
}