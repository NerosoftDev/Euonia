using System.Collections.Concurrent;

namespace System;

public class Singleton<T> where T : class
{
    private static readonly ConcurrentDictionary<Type, T> _container = new();

    /// <summary>
    /// Gets or sets the instance.
    /// </summary>
    public static T Instance
    {
        get => _container[typeof(T)];
        set => _container.AddOrUpdate(typeof(T), _ => value, (_, _) => value);
    }

    public static T Get(Func<T> factory)
    {
        return _container.GetOrAdd(typeof(T), factory);
    }
}
