using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using Nerosoft.Euonia.Collections;

public static partial class Extensions
{
    private static readonly Random _random = new();

    /// <summary>
    /// Performs the specified action on each element of the <see cref="IEnumerable{T}"/>
    /// </summary>
    /// <typeparam name="T">The type of the element.</typeparam>
    /// <param name="source">The source.</param>
    /// <param name="action">The System.Action`1 delegate to perform on each element of the <see cref="IEnumerable{T}"/>.</param>
    /// <exception cref="NullReferenceException">Throws if <paramref name="source"/> is null.</exception>
    /// <exception cref="ArgumentNullException">Throws if action is null.</exception>
    public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
    {
        if (source == null)
        {
            throw new NullReferenceException();
        }

        if (action == null)
        {
            throw new ArgumentNullException(nameof(action));
        }

        foreach (var value in source)
        {
            action(value);
        }
    }

    /// <summary>
    /// Determines whether the collection contains the object.
    /// </summary>
    /// <param name="source">The source.</param>
    /// <param name="value">The value.</param>
    /// <param name="comparison">The comparison.</param>
    /// <returns><c>true</c> if [contains] [the specified value]; otherwise, <c>false</c>.</returns>
    /// <exception cref="NullReferenceException"></exception>
    public static bool Contains(this IEnumerable<string> source, string value, StringComparison comparison)
    {
        if (source == null)
        {
            throw new NullReferenceException();
        }

        return source.Any(t => t.Equals(value, comparison));
    }

    /// <summary>
    /// Determines whether the specified collection [is null or empty].
    /// </summary>
    /// <param name="source">The source.</param>
    /// <returns><c>true</c> if the specified source [is null or empty]; otherwise, <c>false</c>.</returns>
    public static bool IsNullOrEmpty(this IEnumerable source)
    {
        if (source == null)
        {
            return true;
        }

        return !source.GetEnumerator().MoveNext();
    }

    /// <summary>
    /// Determines whether a sequence is null or empty.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source">A sequence in which to locate a value.</param>
    /// <returns></returns>
    public static bool IsNullOrEmpty<T>(this IEnumerable<T> source)
    {
        return source == null || !source.Any();
    }

    /// <summary>
    /// Determine whether the specified collection is equals to another.
    /// </summary>
    /// <typeparam name="T">The type of the collection item.</typeparam>
    /// <param name="source">The source.</param>
    /// <param name="dest">The dest.</param>
    /// <returns><c>true</c> if collection is equals to another, <c>false</c> otherwise.</returns>
    /// <exception cref="NullReferenceException"></exception>
    /// <exception cref="ArgumentNullException">dest</exception>
    public static bool Equals<T>(this IEnumerable<T> source, IEnumerable<T> dest) where T : IComparable
    {
        if (source == null)
        {
            throw new NullReferenceException();
        }

        if (dest == null)
        {
            throw new ArgumentNullException(nameof(dest));
        }

        return dest.Count() == source.Count() && source.All(dest.Contains);
    }

    /// <summary>
    /// Concatenates the elements of an object array, using the specified separator between each element.
    /// </summary>
    /// <typeparam name="T">Member type.</typeparam>
    /// <param name="values">A collection that contains the objects to concatenate.</param>
    /// <param name="separator"></param>
    /// <returns></returns>
    public static string Join<T>(this IEnumerable<T> values, string separator)
    {
        if (values == null)
        {
            throw new NullReferenceException();
        }

        return string.Join(separator, values);
    }

    /// <summary>
    /// Concatenates the members of a collection, using the specified separator between each member.
    /// </summary>
    /// <typeparam name="T">Member type.</typeparam>
    /// <param name="values">A collection that contains the objects to concatenate.</param>
    /// <param name="separator">The string to use as a separator.separator is included in the returned string only if values has more than one element.</param>
    /// <param name="startIndex"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public static string Join<T>(this IEnumerable<T> values, string separator, int startIndex, int count)
    {
        if (values == null)
        {
            throw new NullReferenceException();
        }

        if (startIndex >= values.Count())
        {
            throw new IndexOutOfRangeException();
        }

        return values.Skip(startIndex).Take(count).Join(separator);
    }

    /// <summary>
    /// Convert Pageable collection to view collection.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="source"></param>
    /// <returns></returns>
    public static ViewCollection<T> ToView<T>(this PageableCollection<T> source) where T : class, new()
    {
        if (source == null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        return new ViewCollection<T>(source.ToArray(), source.TotalCount);
    }

    /// <summary>
    /// Read <see cref="IList{T}"/> to a pageable collection.
    /// </summary>
    /// <typeparam name="T">The type of the element.</typeparam>
    /// <param name="source">The source.</param>
    /// <param name="totalCount">The total count.</param>
    /// <param name="index">The index.</param>
    /// <param name="size">The size.</param>
    /// <returns>A new pageable collection contains all elements of <paramref name="source"/>.</returns>
    /// <exception cref="NullReferenceException">Throws if source is null.</exception>
    public static PageableCollection<T> Paginate<T>(this IList<T> source, long totalCount, int index, int size)
    {
        if (source == null)
        {
            throw new NullReferenceException();
        }

        return new PageableCollection<T>(source) { TotalCount = totalCount, PageNumber = index, PageSize = size };
    }

    /// <summary>
    /// Converts an exists pageable collection to another.
    /// </summary>
    /// <typeparam name="T">The type of collection items.</typeparam>
    /// <param name="source">The source.</param>
    /// <param name="index">The index.</param>
    /// <param name="size">The size.</param>
    /// <returns>A new pageable collection.</returns>
    /// <exception cref="ArgumentNullException">Throw if source is null.</exception>
    public static PageableCollection<T> Convert<T>(this PageableCollection<T> source, int index, int size)
    {
        if (source == null)
        {
            throw new ArgumentNullException(nameof(source));
        }

        return new PageableCollection<T>(source) { TotalCount = source.TotalCount, PageNumber = index, PageSize = size };
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="enumerable"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> enumerable)
    {
        var buffer = enumerable.ToList();

        for (var i = 0; i < buffer.Count; i++)
        {
            var j = _random.Next(i, buffer.Count);

            yield return buffer[j];

            buffer[j] = buffer[i];
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="source"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static ObservableCollection<T> ToObservable<T>(this IEnumerable<T> source)
    {
        var collection = new ObservableCollection<T>(source);
        return collection;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="source"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static IReadOnlyCollection<T> Reify<T>(this IEnumerable<T> source)
    {
        return source switch
        {
            null => throw new ArgumentNullException(nameof(source)),
            IReadOnlyCollection<T> result => result,
            ICollection<T> collection => new CollectionWrapper<T>(collection),
            ICollection nonGenericCollection => new NonGenericCollectionWrapper<T>(nonGenericCollection),
            _ => new List<T>(source)
        };
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="source"></param>
    /// <param name="index"></param>
    /// <param name="items"></param>
    /// <typeparam name="T"></typeparam>
    public static void InsertRange<T>(this IList<T> source, int index, IEnumerable<T> items)
    {
        foreach (var item in items)
        {
            source.Insert(index++, item);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="source"></param>
    /// <param name="selector"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static int FindIndex<T>(this IList<T> source, Predicate<T> selector)
    {
        for (var i = 0; i < source.Count; ++i)
        {
            if (selector(source[i]))
            {
                return i;
            }
        }

        return -1;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="source"></param>
    /// <param name="item"></param>
    /// <typeparam name="T"></typeparam>
    public static void AddFirst<T>(this IList<T> source, T item)
    {
        source.Insert(0, item);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="source"></param>
    /// <param name="item"></param>
    /// <typeparam name="T"></typeparam>
    public static void AddLast<T>(this IList<T> source, T item)
    {
        source.Insert(source.Count, item);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="source"></param>
    /// <param name="existingItem"></param>
    /// <param name="item"></param>
    /// <typeparam name="T"></typeparam>
    public static void InsertAfter<T>(this IList<T> source, T existingItem, T item)
    {
        var index = source.IndexOf(existingItem);
        if (index < 0)
        {
            source.AddFirst(item);
            return;
        }

        source.Insert(index + 1, item);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="source"></param>
    /// <param name="selector"></param>
    /// <param name="item"></param>
    /// <typeparam name="T"></typeparam>
    public static void InsertAfter<T>(this IList<T> source, Predicate<T> selector, T item)
    {
        var index = source.FindIndex(selector);
        if (index < 0)
        {
            source.AddFirst(item);
            return;
        }

        source.Insert(index + 1, item);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="source"></param>
    /// <param name="existingItem"></param>
    /// <param name="item"></param>
    /// <typeparam name="T"></typeparam>
    public static void InsertBefore<T>(this IList<T> source, T existingItem, T item)
    {
        var index = source.IndexOf(existingItem);
        if (index < 0)
        {
            source.AddLast(item);
            return;
        }

        source.Insert(index, item);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="source"></param>
    /// <param name="selector"></param>
    /// <param name="item"></param>
    /// <typeparam name="T"></typeparam>
    public static void InsertBefore<T>(this IList<T> source, Predicate<T> selector, T item)
    {
        var index = source.FindIndex(selector);
        if (index < 0)
        {
            source.AddLast(item);
            return;
        }

        source.Insert(index, item);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="source"></param>
    /// <param name="selector"></param>
    /// <param name="item"></param>
    /// <typeparam name="T"></typeparam>
    public static void ReplaceWhile<T>(this IList<T> source, Predicate<T> selector, T item)
    {
        for (var i = 0; i < source.Count; i++)
        {
            if (selector(source[i]))
            {
                source[i] = item;
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="source"></param>
    /// <param name="selector"></param>
    /// <param name="itemFactory"></param>
    /// <typeparam name="T"></typeparam>
    public static void ReplaceWhile<T>(this IList<T> source, Predicate<T> selector, Func<T, T> itemFactory)
    {
        for (var i = 0; i < source.Count; i++)
        {
            var item = source[i];
            if (selector(item))
            {
                source[i] = itemFactory(item);
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="source"></param>
    /// <param name="selector"></param>
    /// <param name="item"></param>
    /// <typeparam name="T"></typeparam>
    public static void ReplaceOne<T>(this IList<T> source, Predicate<T> selector, T item)
    {
        for (var i = 0; i < source.Count; i++)
        {
            if (selector(source[i]))
            {
                source[i] = item;
                return;
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="source"></param>
    /// <param name="selector"></param>
    /// <param name="itemFactory"></param>
    /// <typeparam name="T"></typeparam>
    public static void ReplaceOne<T>(this IList<T> source, Predicate<T> selector, Func<T, T> itemFactory)
    {
        for (var i = 0; i < source.Count; i++)
        {
            var item = source[i];
            if (!selector(item))
            {
                continue;
            }

            source[i] = itemFactory(item);
            return;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="source"></param>
    /// <param name="item"></param>
    /// <param name="replaceWith"></param>
    /// <typeparam name="T"></typeparam>
    public static void ReplaceOne<T>(this IList<T> source, T item, T replaceWith)
    {
        for (var i = 0; i < source.Count; i++)
        {
            if (Comparer<T>.Default.Compare(source[i], item) != 0)
            {
                continue;
            }

            source[i] = replaceWith;
            return;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="source"></param>
    /// <param name="selector"></param>
    /// <param name="targetIndex"></param>
    /// <typeparam name="T"></typeparam>
    /// <exception cref="IndexOutOfRangeException"></exception>
    public static void MoveItem<T>(this List<T> source, Predicate<T> selector, int targetIndex)
    {
        if (!targetIndex.IsBetween(0, source.Count - 1))
        {
            throw new IndexOutOfRangeException("targetIndex should be between 0 and " + (source.Count - 1));
        }

        var currentIndex = source.FindIndex(0, selector);
        if (currentIndex == targetIndex)
        {
            return;
        }

        var item = source[currentIndex];
        source.RemoveAt(currentIndex);
        source.Insert(targetIndex, item);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="source"></param>
    /// <param name="selector"></param>
    /// <param name="factory"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T GetOrAdd<T>([NotNull] this IList<T> source, Func<T, bool> selector, Func<T> factory)
    {
        Check.EnsureNotNull(source, nameof(source));

        var item = source.FirstOrDefault(selector);

        if (item == null)
        {
            item = factory();
            source.Add(item);
        }

        return item;
    }

    /// <summary>
    /// Sort a list by a topological sorting, which consider their dependencies.
    /// </summary>
    /// <typeparam name="T">The type of the members of values.</typeparam>
    /// <param name="source">A list of objects to sort</param>
    /// <param name="getDependencies">Function to resolve the dependencies</param>
    /// <param name="comparer">Equality comparer for dependencies </param>
    /// <returns>
    /// Returns a new list ordered by dependencies.
    /// If A depends on B, then B will come before than A in the resulting list.
    /// </returns>
    public static List<T> SortByDependencies<T>(
        this IEnumerable<T> source,
        Func<T, IEnumerable<T>> getDependencies,
        IEqualityComparer<T> comparer = null)
    {
        /* See: http://www.codeproject.com/Articles/869059/Topological-sorting-in-Csharp
         *      http://en.wikipedia.org/wiki/Topological_sorting
         */

        var sorted = new List<T>();
        var visited = new Dictionary<T, bool>(comparer);

        foreach (var item in source)
        {
            SortByDependenciesVisit(item, getDependencies, sorted, visited);
        }

        return sorted;
    }

    /// <summary>
    ///
    /// </summary>
    /// <typeparam name="T">The type of the members of values.</typeparam>
    /// <param name="item">Item to resolve</param>
    /// <param name="getDependencies">Function to resolve the dependencies</param>
    /// <param name="sorted">List with the sorted items</param>
    /// <param name="visited">Dictionary with the visited items</param>
    private static void SortByDependenciesVisit<T>(T item, Func<T, IEnumerable<T>> getDependencies, IList<T> sorted, Dictionary<T, bool> visited)
    {
        var alreadyVisited = visited.TryGetValue(item, out var inProcess);

        if (alreadyVisited)
        {
            if (inProcess)
            {
                throw new ArgumentException("Cyclic dependency found! Item: " + item);
            }
        }
        else
        {
            visited[item] = true;

            var dependencies = getDependencies(item);
            if (dependencies != null)
            {
                foreach (var dependency in dependencies)
                {
                    SortByDependenciesVisit(dependency, getDependencies, sorted, visited);
                }
            }

            visited[item] = false;
            sorted.Add(item);
        }
    }

    /// <summary>
    /// Concatenates the members of a constructed <see cref="IEnumerable{T}"/> collection of type System.String, using the specified separator between each member.
    /// This is a shortcut for string.Join(...)
    /// </summary>
    /// <param name="source">A collection that contains the strings to concatenate.</param>
    /// <param name="separator">The string to use as a separator. separator is included in the returned string only if values has more than one element.</param>
    /// <returns>A string that consists of the members of values delimited by the separator string. If values has no members, the method returns System.String.Empty.</returns>
    public static string JoinAsString(this IEnumerable<string> source, string separator)
    {
        return string.Join(separator, source);
    }

    /// <summary>
    /// Concatenates the members of a collection, using the specified separator between each member.
    /// This is a shortcut for string.Join(...)
    /// </summary>
    /// <param name="source">A collection that contains the objects to concatenate.</param>
    /// <param name="separator">The string to use as a separator. separator is included in the returned string only if values has more than one element.</param>
    /// <typeparam name="T">The type of the members of values.</typeparam>
    /// <returns>A string that consists of the members of values delimited by the separator string. If values has no members, the method returns System.String.Empty.</returns>
    public static string JoinAsString<T>(this IEnumerable<T> source, string separator)
    {
        return string.Join(separator, source);
    }

    /// <summary>
    /// Filters a <see cref="IEnumerable{T}"/> by given predicate if given condition is true.
    /// </summary>
    /// <param name="source">Enumerable to apply filtering</param>
    /// <param name="condition">A boolean value</param>
    /// <param name="predicate">Predicate to filter the enumerable</param>
    /// <returns>Filtered or not filtered enumerable based on <paramref name="condition"/></returns>
    public static IEnumerable<T> WhereIf<T>(this IEnumerable<T> source, bool condition, Func<T, bool> predicate)
    {
        return condition
            ? source.Where(predicate)
            : source;
    }

    /// <summary>
    /// Filters a <see cref="IEnumerable{T}"/> by given predicate if given condition is true.
    /// </summary>
    /// <param name="source">Enumerable to apply filtering</param>
    /// <param name="condition">A boolean value</param>
    /// <param name="predicate">Predicate to filter the enumerable</param>
    /// <returns>Filtered or not filtered enumerable based on <paramref name="condition"/></returns>
    public static IEnumerable<T> WhereIf<T>(this IEnumerable<T> source, bool condition, Func<T, int, bool> predicate)
    {
        return condition
            ? source.Where(predicate)
            : source;
    }

    /// <summary>
    /// This method is used to try to get a value in a dictionary if it does exists.
    /// </summary>
    /// <typeparam name="T">Type of the value</typeparam>
    /// <param name="dictionary">The collection object</param>
    /// <param name="key">Key</param>
    /// <param name="value">Value of the key (or default value if key not exists)</param>
    /// <returns>True if key does exists in the dictionary</returns>
    internal static bool TryGetValue<T>(this IDictionary<string, object> dictionary, string key, out T value)
    {
        if (dictionary.TryGetValue(key, out var valueObj) && valueObj is T result)
        {
            value = result;
            return true;
        }

        value = default;
        return false;
    }

    /// <summary>
    /// Gets a value from the dictionary with given key. Returns default value if can not find.
    /// </summary>
    /// <param name="dictionary">Dictionary to check and get</param>
    /// <param name="key">Key to find the value</param>
    /// <typeparam name="TKey">Type of the key</typeparam>
    /// <typeparam name="TValue">Type of the value</typeparam>
    /// <returns>Value if found, default if can not found.</returns>
    public static TValue GetOrDefault<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key)
    {
        return dictionary.TryGetValue(key, out var obj) ? obj : default;
    }

    /// <summary>
    /// Gets a value from the dictionary with given key. Returns default value if can not find.
    /// </summary>
    /// <param name="dictionary">Dictionary to check and get</param>
    /// <param name="key">Key to find the value</param>
    /// <typeparam name="TKey">Type of the key</typeparam>
    /// <typeparam name="TValue">Type of the value</typeparam>
    /// <returns>Value if found, default if can not found.</returns>
    public static TValue GetOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
    {
        return dictionary.TryGetValue(key, out var obj) ? obj : default;
    }

    /// <summary>
    /// Gets a value from the dictionary with given key. Returns default value if can not find.
    /// </summary>
    /// <param name="dictionary">Dictionary to check and get</param>
    /// <param name="key">Key to find the value</param>
    /// <typeparam name="TKey">Type of the key</typeparam>
    /// <typeparam name="TValue">Type of the value</typeparam>
    /// <returns>Value if found, default if can not found.</returns>
    public static TValue GetOrDefault<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> dictionary, TKey key)
    {
        return dictionary.TryGetValue(key, out var obj) ? obj : default;
    }

    /// <summary>
    /// Gets a value from the dictionary with given key. Returns default value if can not find.
    /// </summary>
    /// <param name="dictionary">Dictionary to check and get</param>
    /// <param name="key">Key to find the value</param>
    /// <typeparam name="TKey">Type of the key</typeparam>
    /// <typeparam name="TValue">Type of the value</typeparam>
    /// <returns>Value if found, default if can not found.</returns>
    public static TValue GetOrDefault<TKey, TValue>(this ConcurrentDictionary<TKey, TValue> dictionary, TKey key)
    {
        return dictionary.TryGetValue(key, out var obj) ? obj : default;
    }

    /// <summary>
    /// Gets a value from the dictionary with given key. Returns default value if can not find.
    /// </summary>
    /// <param name="dictionary">Dictionary to check and get</param>
    /// <param name="key">Key to find the value</param>
    /// <param name="factory">A factory method used to create the value if not found in the dictionary</param>
    /// <typeparam name="TKey">Type of the key</typeparam>
    /// <typeparam name="TValue">Type of the value</typeparam>
    /// <returns>Value if found, default if can not found.</returns>
    public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, Func<TKey, TValue> factory)
    {
        if (dictionary.TryGetValue(key, out var obj))
        {
            return obj;
        }

        return dictionary[key] = factory(key);
    }

    /// <summary>
    /// Gets a value from the dictionary with given key. Returns default value if can not find.
    /// </summary>
    /// <param name="dictionary">Dictionary to check and get</param>
    /// <param name="key">Key to find the value</param>
    /// <param name="factory">A factory method used to create the value if not found in the dictionary</param>
    /// <typeparam name="TKey">Type of the key</typeparam>
    /// <typeparam name="TValue">Type of the value</typeparam>
    /// <returns>Value if found, default if can not found.</returns>
    public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, Func<TValue> factory)
    {
        return dictionary.GetOrAdd(key, _ => factory());
    }

    /// <summary>
    /// Adds an item to the collection if it's not already in the collection.
    /// </summary>
    /// <param name="source">The collection</param>
    /// <param name="item">Item to check and add</param>
    /// <typeparam name="T">Type of the items in the collection</typeparam>
    /// <returns>Returns True if added, returns False if not.</returns>
    public static bool AddIfNotContains<T>([NotNull] this ICollection<T> source, T item)
    {
        Check.EnsureNotNull(source, nameof(source));

        if (source.Contains(item))
        {
            return false;
        }

        source.Add(item);
        return true;
    }

    /// <summary>
    /// Adds items to the collection which are not already in the collection.
    /// </summary>
    /// <param name="source">The collection</param>
    /// <param name="items">Item to check and add</param>
    /// <typeparam name="T">Type of the items in the collection</typeparam>
    /// <returns>Returns the added items.</returns>
    public static IEnumerable<T> AddIfNotContains<T>([NotNull] this ICollection<T> source, IEnumerable<T> items)
    {
        Check.EnsureNotNull(source, nameof(source));

        var addedItems = new List<T>();

        foreach (var item in items)
        {
            if (source.Contains(item))
            {
                continue;
            }

            source.Add(item);
            addedItems.Add(item);
        }

        return addedItems;
    }

    /// <summary>
    /// Adds an item to the collection if it's not already in the collection based on the given <paramref name="predicate"/>.
    /// </summary>
    /// <param name="source">The collection</param>
    /// <param name="predicate">The condition to decide if the item is already in the collection</param>
    /// <param name="itemFactory">A factory that returns the item</param>
    /// <typeparam name="T">Type of the items in the collection</typeparam>
    /// <returns>Returns True if added, returns False if not.</returns>
    public static bool AddIfNotContains<T>([NotNull] this ICollection<T> source, [NotNull] Func<T, bool> predicate, [NotNull] Func<T> itemFactory)
    {
        Check.EnsureNotNull(source, nameof(source));
        Check.EnsureNotNull(predicate, nameof(predicate));
        Check.EnsureNotNull(itemFactory, nameof(itemFactory));

        if (source.Any(predicate))
        {
            return false;
        }

        source.Add(itemFactory());
        return true;
    }

    /// <summary>
    /// Removes all items from the collection those satisfy the given <paramref name="predicate"/>.
    /// </summary>
    /// <typeparam name="T">Type of the items in the collection</typeparam>
    /// <param name="source">The collection</param>
    /// <param name="predicate">The condition to remove the items</param>
    /// <returns>List of removed items</returns>
    public static IList<T> RemoveAll<T>([NotNull] this ICollection<T> source, Func<T, bool> predicate)
    {
        var items = source.Where(predicate).ToList();

        foreach (var item in items)
        {
            source.Remove(item);
        }

        return items;
    }

    /// <summary>
    /// Removes all items from the collection those satisfy the given <paramref>
    ///     <name>predicate</name>
    /// </paramref>
    /// .
    /// </summary>
    /// <typeparam name="T">Type of the items in the collection</typeparam>
    /// <param name="source">The collection</param>
    /// <param name="items">Items to be removed from the list</param>
    public static void RemoveAll<T>([NotNull] this ICollection<T> source, IEnumerable<T> items)
    {
        foreach (var item in items)
        {
            source.Remove(item);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="dictionary"></param>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <returns></returns>
    public static IDictionary<TKey, TValue> Set<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue value)
    {
        dictionary[key] = value;
        return dictionary;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="dictionary"></param>
    /// <param name="key"></param>
    /// <param name="func"></param>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <typeparam name="TRef"></typeparam>
    public static void TryGetValue<TKey, TValue, TRef>(this IDictionary<TKey, TValue> dictionary, TKey key, Action<TRef> func)
    {
        if (!dictionary.TryGetValue(key, out var value))
        {
            return;
        }

        var refValue = (TRef)System.Convert.ChangeType(value, typeof(TRef));
        func(refValue);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="dictionary"></param>
    /// <param name="key"></param>
    /// <param name="func"></param>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TRef"></typeparam>
    public static void TryGetValue<TKey, TRef>(this IDictionary<TKey, object> dictionary, TKey key, Action<TRef> func)
    {
        if (!dictionary.TryGetValue(key, out var value))
        {
            return;
        }

        var refValue = (TRef)value;
        func(refValue);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="dictionary"></param>
    /// <param name="key"></param>
    /// <param name="func"></param>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public static void TryGetValue<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, Action<TValue> func)
    {
        if (!dictionary.TryGetValue(key, out var value))
        {
            return;
        }

        func(value);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="dictionary"></param>
    /// <param name="key"></param>
    /// <param name="comparison"></param>
    /// <typeparam name="TValue"></typeparam>
    /// <returns></returns>
    public static TValue GetValue<TValue>(this IDictionary<string, TValue> dictionary, string key, StringComparison comparison)
    {
        var item = dictionary.FirstOrDefault(t => t.Key.Equals(key, comparison));
        return item.Value;
    }

    /// <summary>
    /// Try get value for specified key from dictionary.
    /// </summary>
    /// <typeparam name="TKey">The key type.</typeparam>
    /// <typeparam name="TValue">The value type.</typeparam>
    /// <param name="source"></param>
    /// <param name="key"></param>
    /// <returns></returns>
    public static TValue TryGetValue<TKey, TValue>(this IDictionary<TKey, TValue> source, TKey key)
    {
        if (source == null)
        {
            throw new NullReferenceException();
        }

        return source.TryGetValue(key, out var value) ? value : default;
    }

    /// <summary>
    /// Try get value for specified key from dictionary.
    /// </summary>
    /// <typeparam name="TKey">The key type.</typeparam>
    /// <typeparam name="TValue">The value type.</typeparam>
    /// <param name="source"></param>
    /// <param name="key"></param>
    /// <param name="defaultValue">The defaut value if key doesn't exists.</param>
    /// <returns></returns>
    /// <exception cref="NullReferenceException"></exception>
    public static TValue TryGetValue<TKey, TValue>(this IDictionary<TKey, TValue> source, TKey key, TValue defaultValue)
    {
        if (source == null)
        {
            throw new NullReferenceException();
        }

        return source.TryGetValue(key, out var value) ? value : defaultValue;
    }

    /// <summary>
    /// Try get exists value for specified key or set value if not exists.
    /// </summary>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="source">The source.</param>
    /// <param name="key">The key.</param>
    /// <returns>The value.</returns>
    /// <exception cref="NullReferenceException">Throws if <paramref name="source"/> is null.</exception>
    public static TValue TryGetOrSetValue<TKey, TValue>(this IDictionary<TKey, TValue> source, TKey key)
    {
        if (source == null)
        {
            throw new NullReferenceException();
        }

        if (source.TryGetValue(key, out var value))
        {
            return value;
        }

        source.Add(key, value);
        return source[key];
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="source"></param>
    /// <param name="key"></param>
    /// <param name="defaultValue"></param>
    /// <param name="comparison"></param>
    /// <typeparam name="TValue"></typeparam>
    /// <returns></returns>
    /// <exception cref="NullReferenceException"></exception>
    public static TValue TryGetValue<TValue>(this IDictionary<string, TValue> source, string key, TValue defaultValue, StringComparison comparison)
    {
        if (source == null)
        {
            throw new NullReferenceException();
        }

        return source.Keys.Contains(key, comparison) ? source.FirstOrDefault(t => t.Key.Equals(key, comparison)).Value : defaultValue;
    }

    /// <summary>
    /// Gets index of item.
    /// </summary>
    /// <param name="enumerable"></param>
    /// <param name="item"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static int IndexOf<T>(this IEnumerable<T> enumerable, T item)
    {
        if (enumerable == null)
            throw new ArgumentNullException(nameof(enumerable));

        var i = 0;
        foreach (var element in enumerable)
        {
            if (Equals(element, item))
            {
                return i;
            }

            i++;
        }

        return -1;
    }

    /// <summary>
    /// Gets index of item which matches the predicate.
    /// </summary>
    /// <param name="enumerable"></param>
    /// <param name="predicate"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static int IndexOf<T>(this IEnumerable<T> enumerable, Func<T, bool> predicate)
    {
        var i = 0;
        foreach (var element in enumerable)
        {
            if (predicate(element))
            {
                return i;
            }

            i++;
        }

        return -1;
    }
}