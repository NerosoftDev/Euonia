using System.Collections.Concurrent;
using System.Diagnostics;

namespace Nerosoft.Euonia.Caching.Internal;

/// <summary>
/// This handle is for internal use and testing. It does not implement any expiration.
/// </summary>
/// <typeparam name="TValue">The type of the cache value.</typeparam>
public class DictionaryCacheHandle<TValue> : BaseCacheHandle<TValue>
{
    private const int ScanInterval = 5000;
    private readonly static Random _random = new();
    private readonly ConcurrentDictionary<string, CacheItem<TValue>> _cache;
    private readonly Timer _timer;

    //private long _lastScan = 0L;
    private int _scanRunning;

    //private object _startScanLock = new object();

    /// <summary>
    /// Initializes a new instance of the <see cref="DictionaryCacheHandle{TCacheValue}"/> class.
    /// </summary>
    /// <param name="managerConfiguration">The manager configuration.</param>
    /// <param name="configuration">The cache handle configuration.</param>
    /// 
    public DictionaryCacheHandle(CacheManagerConfiguration managerConfiguration, CacheHandleConfiguration configuration)
        : base(managerConfiguration, configuration)
    {
        _cache = new ConcurrentDictionary<string, CacheItem<TValue>>();
        _timer = new Timer(TimerLoop, null, _random.Next(1000, ScanInterval), ScanInterval);
    }

    /// <summary>
    /// Gets the count.
    /// </summary>
    /// <value>The count.</value>
    public override int Count => _cache.Count;

    /// <summary>
    /// Clears this cache, removing all items in the base cache and all regions.
    /// </summary>
    public override void Clear() => _cache.Clear();

    /// <summary>
    /// Clears the cache region, removing all items from the specified <paramref name="region"/> only.
    /// </summary>
    /// <param name="region">The cache region.</param>
    /// <exception cref="ArgumentNullException">If region is null.</exception>
    public override void ClearRegion(string region)
    {
        Check.EnsureNotNullOrWhiteSpace(region, nameof(region));

        var key = string.Concat(region, ":");
        foreach (var item in _cache.Where(p => p.Key.StartsWith(key, StringComparison.OrdinalIgnoreCase)))
        {
            _cache.TryRemove(item.Key, out _);
        }
    }

    /// <inheritdoc />
    public override bool Exists(string key)
    {
        Check.EnsureNotNullOrWhiteSpace(key, nameof(key));

        return _cache.ContainsKey(key);
    }

    /// <inheritdoc />
    public override bool Exists(string key, string region)
    {
        Check.EnsureNotNullOrWhiteSpace(region, nameof(region));
        var fullKey = GetKey(key, region);
        return _cache.ContainsKey(fullKey);
    }

    /// <summary>
    /// Adds a value to the cache.
    /// </summary>
    /// <param name="item">The <c>CacheItem</c> to be added to the cache.</param>
    /// <returns>
    /// <c>true</c> if the key was not already added to the cache, <c>false</c> otherwise.
    /// </returns>
    /// <exception cref="ArgumentNullException">If item is null.</exception>
    protected override bool AddInternalPrepared(CacheItem<TValue> item)
    {
        Check.EnsureNotNull(item, nameof(item));

        var key = GetKey(item.Key, item.Region);

        return _cache.TryAdd(key, item);
    }

    /// <summary>
    /// Gets a <c>CacheItem</c> for the specified key.
    /// </summary>
    /// <param name="key">The key being used to identify the item within the cache.</param>
    /// <returns>The <c>CacheItem</c>.</returns>
    protected override CacheItem<TValue> GetCacheItemInternal(string key) =>
        GetCacheItemInternal(key, null);

    /// <summary>
    /// Gets a <c>CacheItem</c> for the specified key.
    /// </summary>
    /// <param name="key">The key being used to identify the item within the cache.</param>
    /// <param name="region">The cache region.</param>
    /// <returns>The <c>CacheItem</c>.</returns>
    protected override CacheItem<TValue> GetCacheItemInternal(string key, string region)
    {
        var fullKey = GetKey(key, region);

        if (_cache.TryGetValue(fullKey, out CacheItem<TValue> result))
        {
            if (result.ExpirationMode != CacheExpirationMode.None && IsExpired(result, DateTime.UtcNow))
            {
                _cache.TryRemove(fullKey, out _);
                TriggerCacheSpecificRemove(key, region, CacheItemRemovedReason.Expired, result.Value);
                return null;
            }
        }

        return result;
    }

    /// <summary>
    /// Puts the <paramref name="item"/> into the cache. If the item exists it will get updated
    /// with the new value. If the item doesn't exist, the item will be added to the cache.
    /// </summary>
    /// <param name="item">The <c>CacheItem</c> to be added to the cache.</param>
    /// <exception cref="ArgumentNullException">If item is null.</exception>
    protected override void PutInternalPrepared(CacheItem<TValue> item)
    {
        Check.EnsureNotNull(item, nameof(item));

        _cache[GetKey(item.Key, item.Region)] = item;
    }

    /// <summary>
    /// Removes a value from the cache for the specified key.
    /// </summary>
    /// <param name="key">The key being used to identify the item within the cache.</param>
    /// <returns>
    /// <c>true</c> if the key was found and removed from the cache, <c>false</c> otherwise.
    /// </returns>
    protected override bool RemoveInternal(string key) => RemoveInternal(key, null);

    /// <summary>
    /// Removes a value from the cache for the specified key.
    /// </summary>
    /// <param name="key">The key being used to identify the item within the cache.</param>
    /// <param name="region">The cache region.</param>
    /// <returns>
    /// <c>true</c> if the key was found and removed from the cache, <c>false</c> otherwise.
    /// </returns>
    protected override bool RemoveInternal(string key, string region)
    {
        var fullKey = GetKey(key, region);
        return _cache.TryRemove(fullKey, out _);
    }

    /// <summary>
    /// Gets the key.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <param name="region">The region.</param>
    /// <returns>The full key.</returns>
    /// <exception cref="ArgumentException">If Key is empty.</exception>
    private static string GetKey(string key, string region)
    {
        Check.EnsureNotNullOrWhiteSpace(key, nameof(key));

        if (string.IsNullOrWhiteSpace(region))
        {
            return key;
        }

        return string.Concat(region, ":", key);
    }

    private static bool IsExpired(CacheItem<TValue> item, DateTime now)
    {
        if (item.ExpirationMode == CacheExpirationMode.Absolute
            && item.CreatedUtc.Add(item.ExpirationTimeout) < now)
        {
            return true;
        }
        else if (item.ExpirationMode == CacheExpirationMode.Sliding
            && item.LastAccessedUtc.Add(item.ExpirationTimeout) < now)
        {
            return true;
        }

        return false;
    }

    private void TimerLoop(object state)
    {
        if (_scanRunning > 0)
        {
            return;
        }

        if (Interlocked.CompareExchange(ref _scanRunning, 1, 0) == 0)
        {
            try
            {
                ScanForExpiredItems();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                Interlocked.Exchange(ref _scanRunning, 0);
            }
        }
    }

    private int ScanForExpiredItems()
    {
        var removed = 0;
        var now = DateTime.UtcNow;
        foreach (var item in _cache.Values)
        {
            if (IsExpired(item, now))
            {
                RemoveInternal(item.Key, item.Region);

                // trigger global eviction event
                TriggerCacheSpecificRemove(item.Key, item.Region, CacheItemRemovedReason.Expired, item.Value);

                // fix stats
                Stats.OnRemove(item.Region);
                removed++;
            }
        }

        return removed;
    }
}