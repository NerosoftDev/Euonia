using System.Collections.Concurrent;

namespace Nerosoft.Euonia.Caching;

/// <summary>
/// Class CacheManagerExtensions.
/// </summary>
public static class CacheManagerExtensions
{
    /// <summary>
    /// The locks
    /// </summary>
    private static readonly ConcurrentDictionary<object, object> _locks = new();

    public static TResult Get<TResult>(this ICacheManager manager, string key)
    {
        return manager.Get<string, TResult>(key);
    }

    public static TResult GetOrAdd<TResult>(this ICacheManager manager, string key, Func<AcquireContext<string>, TResult> acquire)
    {
        return manager.GetOrAdd(key, acquire);
    }

    public static TResult AddOrUpdate<TResult>(this ICacheManager manager, string key, Func<AcquireContext<string>, TResult> acquire)
    {
        return manager.AddOrUpdate(key, acquire);
    }

    /// <summary>
    /// Gets the specified key.
    /// </summary>
    /// <typeparam name="TKey">The type of the t key.</typeparam>
    /// <typeparam name="TResult">The type of the t result.</typeparam>
    /// <param name="manager">The cache manager.</param>
    /// <param name="key">The key.</param>
    /// <param name="preventConcurrentCalls">if set to <c>true</c> [prevent concurrent calls].</param>
    /// <param name="acquire">The acquire.</param>
    /// <returns>TResult.</returns>
    public static TResult GetOrAdd<TKey, TResult>(this ICacheManager manager, TKey key, Func<AcquireContext<TKey>, TResult> acquire, bool preventConcurrentCalls)
    {
        if (!preventConcurrentCalls)
        {
            return manager.GetOrAdd(key, acquire);
        }

        var lockKey = _locks.GetOrAdd(key, _ => new object());
        lock (lockKey)
        {
            return manager.GetOrAdd(key, acquire);
        }
    }

    public static TResult AddOrUpdate<TKey, TResult>(this ICacheManager manager, TKey key, Func<AcquireContext<TKey>, TResult> acquire, bool preventConcurrentCalls)
    {
        if (!preventConcurrentCalls)
        {
            return manager.AddOrUpdate(key, acquire);
        }

        var lockKey = _locks.GetOrAdd(key, _ => new object());
        lock (lockKey)
        {
            return manager.AddOrUpdate(key, acquire);
        }
    }

    public static TResult GetOrAdd<TResult>(this ICacheManager manager, string key, Func<AcquireContext<string>, TResult> acquire, bool preventConcurrentCalls)
    {
        return manager.GetOrAdd<string, TResult>(key, acquire, preventConcurrentCalls);
    }

    public static TResult AddOrUpdate<TResult>(this ICacheManager manager, string key, Func<AcquireContext<string>, TResult> acquire, bool preventConcurrentCalls)
    {
        return manager.AddOrUpdate<string, TResult>(key, acquire, preventConcurrentCalls);
    }
}