﻿using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using Nerosoft.Euonia.Collections;

public static partial class Extensions
{
    /// <summary>
    /// Return the first group with <paramref name="key"/> key.
    /// </summary>
    /// <typeparam name="TKey">The type of the group key.</typeparam>
    /// <typeparam name="TValue">The type of the items in the collection.</typeparam>
    /// <param name="source">The source <see cref="ObservableGroupedCollection{TKey,TValue}"/> instance.</param>
    /// <param name="key">The key of the group to query.</param>
    /// <returns>The first group matching <paramref name="key"/>.</returns>
    /// <exception cref="InvalidOperationException">The target group does not exist.</exception>
    [Pure]
    public static ObservableGroup<TKey, TValue> First<TKey, TValue>(this ObservableGroupedCollection<TKey, TValue> source, TKey key)
    {
        var group = source.FirstOrDefault(key);

        if (group is null)
        {
            ThrowArgumentExceptionForKeyNotFound();
        }

        return group!;
    }

    /// <summary>
    /// Return the first group with <paramref name="key"/> key or null if not found.
    /// </summary>
    /// <typeparam name="TKey">The type of the group key.</typeparam>
    /// <typeparam name="TValue">The type of the items in the collection.</typeparam>
    /// <param name="source">The source <see cref="ObservableGroupedCollection{TKey, TValue}"/> instance.</param>
    /// <param name="key">The key of the group to query.</param>
    /// <returns>The first group matching <paramref name="key"/> or null.</returns>
    [Pure]
    public static ObservableGroup<TKey, TValue> FirstOrDefault<TKey, TValue>(this ObservableGroupedCollection<TKey, TValue> source, TKey key)
    {
        if (source.TryGetList(out var list))
        {
            foreach (var group in list!)
            {
                if (EqualityComparer<TKey>.Default.Equals(group.Key, key))
                {
                    return group;
                }
            }

            return null;
        }

        return FirstOrDefaultWithLinq(source, key);
    }

    /// <summary>
    /// Slow path for <see cref="First{TKey,TValue}"/>.
    /// </summary>
    [Pure]
    [MethodImpl(MethodImplOptions.NoInlining)]
    private static ObservableGroup<TKey, TValue> FirstOrDefaultWithLinq<TKey, TValue>(
        ObservableGroupedCollection<TKey, TValue> source,
        TKey key)
        => source.FirstOrDefault(group => EqualityComparer<TKey>.Default.Equals(group.Key, key));

    /// <summary>
    /// Return the element at position <paramref name="index"/> from the first group with <paramref name="key"/> key.
    /// </summary>
    /// <typeparam name="TKey">The type of the group key.</typeparam>
    /// <typeparam name="TValue">The type of the items in the collection.</typeparam>
    /// <param name="source">The source <see cref="ObservableGroupedCollection{TKey, TValue}"/> instance.</param>
    /// <param name="key">The key of the group to query.</param>
    /// <param name="index">The index of the item from the targeted group.</param>
    /// <returns>The element.</returns>
    /// <exception cref="InvalidOperationException">The target group does not exist.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is less than zero or <paramref name="index"/> is greater than the group elements' count.</exception>
    [Pure]
    public static TValue ElementAt<TKey, TValue>(
        this ObservableGroupedCollection<TKey, TValue> source,
        TKey key,
        int index)
        => source.First(key)[index];

    /// <summary>
    /// Return the element at position <paramref name="index"/> from the first group with <paramref name="key"/> key.
    /// </summary>
    /// <typeparam name="TKey">The type of the group key.</typeparam>
    /// <typeparam name="TValue">The type of the items in the collection.</typeparam>
    /// <param name="source">The source <see cref="ObservableGroupedCollection{TKey, TValue}"/> instance.</param>
    /// <param name="key">The key of the group to query.</param>
    /// <param name="index">The index of the item from the targeted group.</param>
    /// <returns>The element or default(TValue) if it does not exist.</returns>
    [Pure]
    public static TValue ElementAtOrDefault<TKey, TValue>(
        this ObservableGroupedCollection<TKey, TValue> source,
        TKey key,
        int index)
    {
        var group = source.FirstOrDefault(key);

        if (group is null ||
            (uint)index >= (uint)group.Count)
        {
            return default!;
        }

        return group[index];
    }

    /// <summary>
    /// Adds a key-value <see cref="ObservableGroup{TKey, TValue}"/> item into a target <see cref="ObservableGroupedCollection{TKey, TValue}"/>.
    /// </summary>
    /// <typeparam name="TKey">The type of the group key.</typeparam>
    /// <typeparam name="TValue">The type of the items in the collection.</typeparam>
    /// <param name="source">The source <see cref="ObservableGroupedCollection{TKey, TValue}"/> instance.</param>
    /// <param name="key">The key of the group where <paramref name="value"/> will be added.</param>
    /// <param name="value">The value to add.</param>
    /// <returns>The added <see cref="ObservableGroup{TKey, TValue}"/>.</returns>
    public static ObservableGroup<TKey, TValue> AddGroup<TKey, TValue>(
        this ObservableGroupedCollection<TKey, TValue> source,
        TKey key,
        TValue value)
        => AddGroup(source, key, new[] { value });

    /// <summary>
    /// Adds a key-collection <see cref="ObservableGroup{TKey, TValue}"/> item into a target <see cref="ObservableGroupedCollection{TKey, TValue}"/>.
    /// </summary>
    /// <typeparam name="TKey">The type of the group key.</typeparam>
    /// <typeparam name="TValue">The type of the items in the collection.</typeparam>
    /// <param name="source">The source <see cref="ObservableGroupedCollection{TKey, TValue}"/> instance.</param>
    /// <param name="key">The key of the group where <paramref name="collection"/> will be added.</param>
    /// <param name="collection">The collection to add.</param>
    /// <returns>The added <see cref="ObservableGroup{TKey, TValue}"/>.</returns>
    public static ObservableGroup<TKey, TValue> AddGroup<TKey, TValue>(
        this ObservableGroupedCollection<TKey, TValue> source,
        TKey key,
        params TValue[] collection)
        => source.AddGroup(key, (IEnumerable<TValue>)collection);

    /// <summary>
    /// Adds a key-collection <see cref="ObservableGroup{TKey, TValue}"/> item into a target <see cref="ObservableGroupedCollection{TKey, TValue}"/>.
    /// </summary>
    /// <typeparam name="TKey">The type of the group key.</typeparam>
    /// <typeparam name="TValue">The type of the items in the collection.</typeparam>
    /// <param name="source">The source <see cref="ObservableGroupedCollection{TKey, TValue}"/> instance.</param>
    /// <param name="key">The key of the group where <paramref name="collection"/> will be added.</param>
    /// <param name="collection">The collection to add.</param>
    /// <returns>The added <see cref="ObservableGroup{TKey, TValue}"/>.</returns>
    public static ObservableGroup<TKey, TValue> AddGroup<TKey, TValue>(
        this ObservableGroupedCollection<TKey, TValue> source,
        TKey key,
        IEnumerable<TValue> collection)
    {
        var group = new ObservableGroup<TKey, TValue>(key, collection);
        source.Add(group);

        return group;
    }

    /// <summary>
    /// Add <paramref name="item"/> into the first group with <paramref name="key"/> key.
    /// If the group does not exist, it will be added.
    /// </summary>
    /// <typeparam name="TKey">The type of the group key.</typeparam>
    /// <typeparam name="TValue">The type of the items in the collection.</typeparam>
    /// <param name="source">The source <see cref="ObservableGroupedCollection{TKey, TValue}"/> instance.</param>
    /// <param name="key">The key of the group where the <paramref name="item"/> should be added.</param>
    /// <param name="item">The item to add.</param>
    /// <returns>The instance of the <see cref="ObservableGroup{TKey, TValue}"/> which will receive the value. It will either be an existing group or a new group.</returns>
    public static ObservableGroup<TKey, TValue> AddItem<TKey, TValue>(
        this ObservableGroupedCollection<TKey, TValue> source,
        TKey key,
        TValue item)
    {
        var group = source.FirstOrDefault(key);

        if (group is null)
        {
            group = new ObservableGroup<TKey, TValue>(key);
            source.Add(group);
        }

        group.Add(item);

        return group;
    }

    /// <summary>
    /// Insert <paramref name="item"/> into the first group with <paramref name="key"/> key at <paramref name="index"/>.
    /// </summary>
    /// <typeparam name="TKey">The type of the group key.</typeparam>
    /// <typeparam name="TValue">The type of the items in the collection.</typeparam>
    /// <param name="source">The source <see cref="ObservableGroupedCollection{TKey, TValue}"/> instance.</param>
    /// <param name="key">The key of the group where to insert <paramref name="item"/>.</param>
    /// <param name="index">The index where to insert <paramref name="item"/>.</param>
    /// <param name="item">The item to add.</param>
    /// <returns>The instance of the <see cref="ObservableGroup{TKey, TValue}"/> which will receive the value.</returns>
    /// <exception cref="InvalidOperationException">The target group does not exist.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is less than zero or <paramref name="index"/> is greater than the group elements' count.</exception>
    public static ObservableGroup<TKey, TValue> InsertItem<TKey, TValue>(
        this ObservableGroupedCollection<TKey, TValue> source,
        TKey key,
        int index,
        TValue item)
    {
        var existingGroup = source.First(key);
        existingGroup.Insert(index, item);

        return existingGroup;
    }

    /// <summary>
    /// Replace the element at <paramref name="index"/> with <paramref name="item"/> in the first group with <paramref name="key"/> key.
    /// </summary>
    /// <typeparam name="TKey">The type of the group key.</typeparam>
    /// <typeparam name="TValue">The type of the items in the collection.</typeparam>
    /// <param name="source">The source <see cref="ObservableGroupedCollection{TKey, TValue}"/> instance.</param>
    /// <param name="key">The key of the group where to replace the item.</param>
    /// <param name="index">The index where to insert <paramref name="item"/>.</param>
    /// <param name="item">The item to add.</param>
    /// <returns>The instance of the <see cref="ObservableGroup{TKey, TValue}"/> which will receive the value.</returns>
    /// <exception cref="InvalidOperationException">The target group does not exist.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is less than zero or <paramref name="index"/> is greater than the group elements' count.</exception>
    public static ObservableGroup<TKey, TValue> SetItem<TKey, TValue>(
        this ObservableGroupedCollection<TKey, TValue> source,
        TKey key,
        int index,
        TValue item)
    {
        var existingGroup = source.First(key);
        existingGroup[index] = item;

        return existingGroup;
    }

    /// <summary>
    /// Remove the first occurrence of the group with <paramref name="key"/> from the <paramref name="source"/> grouped collection.
    /// It will not do anything if the group does not exist.
    /// </summary>
    /// <typeparam name="TKey">The type of the group key.</typeparam>
    /// <typeparam name="TValue">The type of the items in the collection.</typeparam>
    /// <param name="source">The source <see cref="ObservableGroupedCollection{TKey, TValue}"/> instance.</param>
    /// <param name="key">The key of the group to remove.</param>
    public static void RemoveGroup<TKey, TValue>(
        this ObservableGroupedCollection<TKey, TValue> source,
        TKey key)
    {
        if (source.TryGetList(out var list))
        {
            var index = 0;
            foreach (var group in list!)
            {
                if (EqualityComparer<TKey>.Default.Equals(group.Key, key))
                {
                    source.RemoveAt(index);

                    return;
                }

                index++;
            }
        }
        else
        {
            RemoveGroupWithLinq(source, key);
        }
    }

    /// <summary>
    /// Slow path for <see cref="RemoveGroup{TKey,TValue}"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void RemoveGroupWithLinq<TKey, TValue>(ObservableGroupedCollection<TKey, TValue> source, TKey key)
    {
        var index = 0;
        foreach (var group in source)
        {
            if (EqualityComparer<TKey>.Default.Equals(group.Key, key))
            {
                source.RemoveAt(index);
                return;
            }

            index++;
        }
    }

    /// <summary>
    /// Remove the first <paramref name="item"/> from the first group with <paramref name="key"/> from the <paramref name="source"/> grouped collection.
    /// It will not do anything if the group or the item does not exist.
    /// </summary>
    /// <typeparam name="TKey">The type of the group key.</typeparam>
    /// <typeparam name="TValue">The type of the items in the collection.</typeparam>
    /// <param name="source">The source <see cref="ObservableGroupedCollection{TKey, TValue}"/> instance.</param>
    /// <param name="key">The key of the group where the <paramref name="item"/> should be removed.</param>
    /// <param name="item">The item to remove.</param>
    /// <param name="removeGroupIfEmpty">If true (default value), the group will be removed once it becomes empty.</param>
    public static void RemoveItem<TKey, TValue>(
        this ObservableGroupedCollection<TKey, TValue> source,
        TKey key,
        TValue item,
        bool removeGroupIfEmpty = true)
    {
        if (source.TryGetList(out var list))
        {
            var index = 0;
            foreach (var group in list!)
            {
                if (EqualityComparer<TKey>.Default.Equals(group.Key, key))
                {
                    if (group.Remove(item) &&
                        removeGroupIfEmpty &&
                        group.Count == 0)
                    {
                        source.RemoveAt(index);
                    }

                    return;
                }

                index++;
            }
        }
        else
        {
            RemoveItemWithLinq(source, key, item, removeGroupIfEmpty);
        }
    }

    /// <summary>
    /// Slow path for <see cref="RemoveItem{TKey,TValue}"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void RemoveItemWithLinq<TKey, TValue>(
        ObservableGroupedCollection<TKey, TValue> source,
        TKey key,
        TValue item,
        bool removeGroupIfEmpty)
    {
        var index = 0;
        foreach (var group in source)
        {
            if (EqualityComparer<TKey>.Default.Equals(group.Key, key))
            {
                if (group.Remove(item) &&
                    removeGroupIfEmpty &&
                    group.Count == 0)
                {
                    source.RemoveAt(index);
                }

                return;
            }

            index++;
        }
    }

    /// <summary>
    /// Remove the item at <paramref name="index"/> from the first group with <paramref name="key"/> from the <paramref name="source"/> grouped collection.
    /// It will not do anything if the group or the item does not exist.
    /// </summary>
    /// <typeparam name="TKey">The type of the group key.</typeparam>
    /// <typeparam name="TValue">The type of the items in the collection.</typeparam>
    /// <param name="source">The source <see cref="ObservableGroupedCollection{TKey, TValue}"/> instance.</param>
    /// <param name="key">The key of the group where the item at <paramref name="index"/> should be removed.</param>
    /// <param name="index">The index of the item to remove in the group.</param>
    /// <param name="removeGroupIfEmpty">If true (default value), the group will be removed once it becomes empty.</param>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is less than zero or <paramref name="index"/> is greater than the group elements' count.</exception>
    public static void RemoveItemAt<TKey, TValue>(
        this ObservableGroupedCollection<TKey, TValue> source,
        TKey key,
        int index,
        bool removeGroupIfEmpty = true)
    {
        if (source.TryGetList(out var list))
        {
            var groupIndex = 0;
            foreach (var group in list!)
            {
                if (EqualityComparer<TKey>.Default.Equals(group.Key, key))
                {
                    group.RemoveAt(index);

                    if (removeGroupIfEmpty && group.Count == 0)
                    {
                        source.RemoveAt(groupIndex);
                    }

                    return;
                }

                groupIndex++;
            }
        }
        else
        {
            RemoveItemAtWithLinq(source, key, index, removeGroupIfEmpty);
        }
    }

    /// <summary>
    /// Slow path for <see cref="RemoveItemAt{TKey,TValue}"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void RemoveItemAtWithLinq<TKey, TValue>(
        ObservableGroupedCollection<TKey, TValue> source,
        TKey key,
        int index,
        bool removeGroupIfEmpty)
    {
        var groupIndex = 0;
        foreach (var group in source)
        {
            if (EqualityComparer<TKey>.Default.Equals(group.Key, key))
            {
                group.RemoveAt(index);

                if (removeGroupIfEmpty && group.Count == 0)
                {
                    source.RemoveAt(groupIndex);
                }

                return;
            }

            groupIndex++;
        }
    }

    /// <summary>
    /// Throws a new <see cref="InvalidOperationException"/> when a key is not found.
    /// </summary>
    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void ThrowArgumentExceptionForKeyNotFound()
    {
        throw new InvalidOperationException("The requested key was not present in the collection");
    }
}
