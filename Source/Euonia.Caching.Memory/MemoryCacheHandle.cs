﻿using System.Collections.Concurrent;
using Microsoft.Extensions.Caching.Memory;
using Nerosoft.Euonia.Caching.Internal;

namespace Nerosoft.Euonia.Caching.Memory;

/// <summary>
/// Implementation of a cache handle using <see cref="Microsoft.Extensions.Caching.Memory"/>.
/// </summary>
/// <typeparam name="TCacheValue">The type of the cache value.</typeparam>
public class MemoryCacheHandle<TCacheValue> : BaseCacheHandle<TCacheValue>
{
    private const string DEFAULT_NAME = "default";

    private readonly string _cacheName = string.Empty;

    private volatile MemoryCache _cache;

    /// <summary>
    /// Initializes a new instance of the <see cref="MemoryCacheHandle{TCacheValue}"/> class.
    /// </summary>
    /// <param name="managerConfiguration">The manager configuration.</param>
    /// <param name="configuration">The cache handle configuration.</param>
    /// 
    public MemoryCacheHandle(CacheManagerConfiguration managerConfiguration, CacheHandleConfiguration configuration)
        : this(managerConfiguration, configuration, null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MemoryCacheHandle{TCacheValue}"/> class.
    /// </summary>
    /// <param name="managerConfiguration">The manager configuration.</param>
    /// <param name="configuration">The cache handle configuration.</param>
    /// <param name="options">The vendor specific options.</param>
    /// 
    public MemoryCacheHandle(CacheManagerConfiguration managerConfiguration, CacheHandleConfiguration configuration, MemoryCacheOptions options)
        : base(managerConfiguration, configuration)
    {
        Check.EnsureNotNull(configuration, nameof(configuration));

        _cacheName = configuration.Name;
        Options = options ?? new MemoryCacheOptions();
        _cache = new MemoryCache(Options);
    }

    /// <inheritdoc/>
    public override int Count => _cache.Count;

    internal MemoryCacheOptions Options { get; }

    /// <inheritdoc/>
    public override void Clear()
    {
        _cache = new MemoryCache(Options);
    }

    /// <inheritdoc/>
    public override void ClearRegion(string region)
    {
        _cache.RemoveChilds(region);
        _cache.Remove(region);
    }

    /// <inheritdoc />
    public override bool Exists(string key)
    {
        return _cache.Contains(GetItemKey(key));
    }

    /// <inheritdoc />
    public override bool Exists(string key, string region)
    {
        Check.EnsureNotNullOrWhiteSpace(region, nameof(region));

        return _cache.Contains(GetItemKey(key, region));
    }

    /// <inheritdoc/>
    protected override CacheItem<TCacheValue> GetCacheItemInternal(string key)
    {
        return GetCacheItemInternal(key, null);
    }

    /// <inheritdoc/>
    protected override CacheItem<TCacheValue> GetCacheItemInternal(string key, string region)
    {
        var fullKey = GetItemKey(key, region);

        if (_cache.Get(fullKey) is not CacheItem<TCacheValue> item)
        {
            return null;
        }

        if (item.IsExpired)
        {
            RemoveInternal(item.Key, item.Region);
            TriggerCacheSpecificRemove(item.Key, item.Region, CacheItemRemovedReason.Expired, item.Value);
            return null;
        }

        if (item.ExpirationMode == CacheExpirationMode.Sliding)
        {
            // item = this.GetItemExpiration(item); // done by basecachehandle already
            _cache.Set(fullKey, item, GetOptions(item));
        }

        return item;
    }

    /// <inheritdoc/>
    protected override bool RemoveInternal(string key)
    {
        return RemoveInternal(key, null);
    }

    /// <inheritdoc/>
    protected override bool RemoveInternal(string key, string region)
    {
        var fullKey = GetItemKey(key, region);
        var result = _cache.Contains(fullKey);
        if (result)
        {
            _cache.Remove(fullKey);
        }

        return result;
    }

    /// <inheritdoc/>
    protected override bool AddInternalPrepared(CacheItem<TCacheValue> item)
    {
        var key = GetItemKey(item);

        if (_cache.Contains(key))
        {
            return false;
        }

        var options = GetOptions(item);
        _cache.Set(key, item, options);

        if (item.Region != null)
        {
            _cache.RegisterChild(item.Region, key);
        }

        return true;
    }

    /// <inheritdoc/>
    protected override void PutInternalPrepared(CacheItem<TCacheValue> item)
    {
        var key = GetItemKey(item);

        var options = GetOptions(item);
        _cache.Set(key, item, options);

        if (item.Region != null)
        {
            _cache.RegisterChild(item.Region, key);
        }
    }

    private string GetItemKey(CacheItem<TCacheValue> item) => GetItemKey(item?.Key, item?.Region);

    private string GetItemKey(string key, string region = null)
    {
        Check.EnsureNotNullOrWhiteSpace(key, nameof(key));

        if (string.IsNullOrWhiteSpace(region))
        {
            return key;
        }

        return region + ":" + key;
    }

    private MemoryCacheEntryOptions GetOptions(CacheItem<TCacheValue> item)
    {
        if (item.Region != null)
        {
            if (!_cache.Contains(item.Region))
            {
                CreateRegionToken(item.Region);
            }
        }

        var options = new MemoryCacheEntryOptions()
            .SetPriority(CacheItemPriority.Normal);

        if (item.ExpirationMode == CacheExpirationMode.Absolute)
        {
            options.SetAbsoluteExpiration(item.ExpirationTimeout);
            options.RegisterPostEvictionCallback(ItemRemoved, Tuple.Create(item.Key, item.Region));
        }

        if (item.ExpirationMode == CacheExpirationMode.Sliding)
        {
            options.SetSlidingExpiration(item.ExpirationTimeout);
            options.RegisterPostEvictionCallback(ItemRemoved, Tuple.Create(item.Key, item.Region));
        }

        item.LastAccessedUtc = DateTime.UtcNow;

        return options;
    }

    private void CreateRegionToken(string region)
    {
        var options = new MemoryCacheEntryOptions
        {
            Priority = CacheItemPriority.Normal,
            AbsoluteExpiration = DateTimeOffset.MaxValue,
            SlidingExpiration = TimeSpan.MaxValue,
        };

        _cache.Set(region, new ConcurrentDictionary<object, bool>(), options);
    }

    private void ItemRemoved(object key, object value, EvictionReason reason, object state)
    {
        var strKey = key as string;
        if (string.IsNullOrWhiteSpace(strKey))
        {
            return;
        }

        // don't trigger stuff on manual remove
        if (reason == EvictionReason.Removed)
        {
            return;
        }

        if (state is Tuple<string, string> keyRegionTupple)
        {
            if (keyRegionTupple.Item2 != null)
            {
                Stats.OnRemove(keyRegionTupple.Item2);
            }
            else
            {
                Stats.OnRemove();
            }

            object originalValue = null;
            if (value is CacheItem<TCacheValue> item)
            {
                originalValue = item.Value;
            }

            if (reason == EvictionReason.Capacity)
            {
                TriggerCacheSpecificRemove(keyRegionTupple.Item1, keyRegionTupple.Item2, CacheItemRemovedReason.Evicted, originalValue);
            }
            else if (reason == EvictionReason.Expired)
            {
                TriggerCacheSpecificRemove(keyRegionTupple.Item1, keyRegionTupple.Item2, CacheItemRemovedReason.Expired, originalValue);
            }
        }
        else
        {
            Stats.OnRemove();
        }
    }
}