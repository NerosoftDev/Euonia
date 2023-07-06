using System.Collections.Concurrent;

namespace System;

/// <summary>
/// Implementation of Singleton design pattern for any class.
/// </summary>
/// <typeparam name="T"></typeparam>
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

    /// <summary>
    /// Gets instance of specified type of <typeparamref name="T"/>
    /// </summary>
    /// <param name="factory">The factory function to create a new instance of <typeparamref name="T"/> if not exists.</param>
    /// <returns></returns>
    public static T Get(Func<T> factory)
    {
        return _container.GetOrAdd(typeof(T), factory);
    }
}