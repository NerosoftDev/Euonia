using System.Collections.Concurrent;

namespace Nerosoft.Euonia.Caching;

/// <summary>
/// Provides the default implementation for a cache holder.
/// The cache holder is responsible for actually storing the references to cached entities.
/// Implements the <see cref="ICacheHolder" />
/// </summary>
/// <seealso cref="ICacheHolder" />
public class DefaultCacheHolder : ICacheHolder
{
    /// <summary>
    /// The cache context accessor
    /// </summary>
    private readonly ICacheContextAccessor _accessor;

    /// <summary>
    /// The caches
    /// </summary>
    private readonly ConcurrentDictionary<CacheKey, object> _caches = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="DefaultCacheHolder"/> class.
    /// </summary>
    /// <param name="accessor">The cache context accessor.</param>
    public DefaultCacheHolder(ICacheContextAccessor accessor)
    {
        _accessor = accessor;
    }

    /// <inheritdoc />
    CacheItem<TKey, TResult> ICacheHolder.GetCache<TKey, TResult>(Type component)
    {
        var key = new CacheKey(component, typeof(TKey), typeof(TResult));
        var result = _caches.GetOrAdd(key, _ => new CacheItem<TKey, TResult>(_accessor));
        return (CacheItem<TKey, TResult>)result;
    }
}