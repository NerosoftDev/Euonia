using System.Collections.ObjectModel;
using System.Globalization;
using Nerosoft.Euonia.Caching.Internal;

namespace Nerosoft.Euonia.Caching;

/// <summary>
/// The <see cref="BaseCacheManager{TCacheValue}"/> implements <see cref="ICacheManager{TCacheValue}"/> and is the main class
/// of this library.
/// The cache manager delegates all cache operations to the list of <see cref="BaseCacheHandle{T}"/>'s which have been
/// added. It will keep them in sync according to rules and depending on the configuration.
/// </summary>
/// <typeparam name="TValue">The type of the cache value.</typeparam>
public partial class BaseCacheManager<TValue> : BaseCache<TValue>, ICacheManager<TValue>
{
    private readonly BaseCacheHandle<TValue>[] _cacheHandles;
    private readonly CacheBackplane _cacheBackplane;

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseCacheManager{TCacheValue}"/> class
    /// using the specified <paramref name="configuration"/>.
    /// If the name of the <paramref name="configuration"/> is defined, the cache manager will
    /// use it. Otherwise a random string will be generated.
    /// </summary>
    /// <param name="configuration">
    /// The configuration which defines the structure and complexity of the cache manager.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// When <paramref name="configuration"/> is null.
    /// </exception>
    /// <see cref="CacheFactory"/>
    /// <see cref="ConfigurationBuilder"/>
    /// <see cref="BaseCacheHandle{TCacheValue}"/>
    public BaseCacheManager(CacheManagerConfiguration configuration)
        : this(configuration?.Name ?? Guid.NewGuid().ToString(), configuration)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseCacheManager{TCacheValue}"/> class
    /// using the specified <paramref name="name"/> and <paramref name="configuration"/>.
    /// </summary>
    /// <param name="name">The cache name.</param>
    /// <param name="configuration">
    /// The configuration which defines the structure and complexity of the cache manager.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// When <paramref name="name"/> or <paramref name="configuration"/> is null.
    /// </exception>
    /// <see cref="CacheFactory"/>
    /// <see cref="ConfigurationBuilder"/>
    /// <see cref="BaseCacheHandle{TCacheValue}"/>
    private BaseCacheManager(string name, CacheManagerConfiguration configuration)
    {
        Check.EnsureNotNullOrWhiteSpace(name, nameof(name));
        Check.EnsureNotNull(configuration, nameof(configuration));

        Name = name;
        Configuration = configuration;

        try
        {
            _cacheHandles = CacheReflectionHelper.CreateCacheHandles(this).ToArray();

            var index = 0;
            foreach (var handle in _cacheHandles)
            {
                var handleIndex = index;
                handle.OnCacheSpecificRemove += (_, args) =>
                {
                    // base cache handle does logging for this

                    if (Configuration.UpdateMode == CacheUpdateMode.Up)
                    {
                        EvictFromHandlesAbove(args.Key, args.Region, handleIndex);
                    }

                    // moving down below cleanup, otherwise the item could still be in memory
                    TriggerOnRemoveByHandle(args.Key, args.Region, args.Reason, handleIndex + 1, args.Value);
                };

                index++;
            }

            _cacheBackplane = CacheReflectionHelper.CreateBackplane(configuration);
            if (_cacheBackplane != null)
            {
                RegisterCacheBackplane(_cacheBackplane);
            }
        }
        catch (Exception ex)
        {
            throw ex.InnerException ?? ex;
        }
    }

    /// <inheritdoc />
    public event EventHandler<CacheActionEventArgs> OnAdd;

    /// <inheritdoc />
    public event EventHandler<CacheClearEventArgs> OnClear;

    /// <inheritdoc />
    public event EventHandler<CacheClearRegionEventArgs> OnClearRegion;

    /// <inheritdoc />
    public event EventHandler<CacheActionEventArgs> OnGet;

    /// <inheritdoc />
    public event EventHandler<CacheActionEventArgs> OnPut;

    /// <inheritdoc />
    public event EventHandler<CacheActionEventArgs> OnRemove;

    /// <inheritdoc />
    public event EventHandler<CacheItemRemovedEventArgs> OnRemoveByHandle;

    /// <inheritdoc />
    public event EventHandler<CacheActionEventArgs> OnUpdate;

    /// <inheritdoc />
    public CacheManagerConfiguration Configuration { get; }

    /// <inheritdoc />
    public IEnumerable<BaseCacheHandle<TValue>> CacheHandles
        => new ReadOnlyCollection<BaseCacheHandle<TValue>>(
            new List<BaseCacheHandle<TValue>>(
                _cacheHandles));

    /// <summary>
    /// Gets the configured cache backplane.
    /// </summary>
    /// <value>The backplane.</value>
    public CacheBackplane Backplane => _cacheBackplane;

    /// <summary>
    /// Gets the cache name.
    /// </summary>
    /// <value>The name of the cache.</value>
    public string Name { get; }

    /// <inheritdoc />
    public override void Clear()
    {
        CheckDisposed();

        foreach (var handle in _cacheHandles)
        {
            handle.Clear();
            handle.Stats.OnClear();
        }

        _cacheBackplane?.NotifyClear();

        TriggerOnClear();
    }

    /// <inheritdoc />
    public override void ClearRegion(string region)
    {
        Check.EnsureNotNullOrWhiteSpace(region, nameof(region));

        CheckDisposed();

        foreach (var handle in _cacheHandles)
        {
            handle.ClearRegion(region);
            handle.Stats.OnClearRegion(region);
        }

        _cacheBackplane?.NotifyClearRegion(region);

        TriggerOnClearRegion(region);
    }

    /// <inheritdoc />
    public override bool Exists(string key)
    {
        foreach (var handle in _cacheHandles)
        {
            if (handle.Exists(key))
            {
                return true;
            }
        }

        return false;
    }

    /// <inheritdoc />
    public override bool Exists(string key, string region)
    {
        foreach (var handle in _cacheHandles)
        {
            if (handle.Exists(key, region))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Returns a <see cref="string" /> that represents this instance.
    /// </summary>
    /// <returns>
    /// A <see cref="string" /> that represents this instance.
    /// </returns>
    public override string ToString() =>
        string.Format(CultureInfo.InvariantCulture, "Name: {0}, Handles: [{1}]", Name, string.Join(",", _cacheHandles.Select(p => p.GetType().Name)));

    /// <inheritdoc />
    protected internal override bool AddInternal(CacheItem<TValue> item)
    {
        Check.EnsureNotNull(item, nameof(item));

        CheckDisposed();

        var handleIndex = _cacheHandles.Length - 1;

        var result = AddItemToHandle(item, _cacheHandles[handleIndex]);

        // evict from other handles in any case because if it exists, it might be a different version
        // if not exist, its just a sanity check to invalidate other versions in upper layers.
        EvictFromOtherHandles(item.Key, item.Region, handleIndex);

        if (result)
        {
            // update backplane
            if (_cacheBackplane != null)
            {
                if (string.IsNullOrWhiteSpace(item.Region))
                {
                    _cacheBackplane.NotifyChange(item.Key, CacheItemChangedEventAction.Add);
                }
                else
                {
                    _cacheBackplane.NotifyChange(item.Key, item.Region, CacheItemChangedEventAction.Add);
                }
            }

            // trigger only once and not per handle and only if the item was added!
            TriggerOnAdd(item.Key, item.Region);
        }

        return result;
    }

    /// <inheritdoc />
    protected internal override void PutInternal(CacheItem<TValue> item)
    {
        Check.EnsureNotNull(item, nameof(item));

        CheckDisposed();

        foreach (var handle in _cacheHandles)
        {
            if (handle.Configuration.EnableStatistics)
            {
                // check if it is really a new item otherwise the items count is crap because we
                // count it every time, but use only the current handle to retrieve the item,
                // otherwise we would trigger gets and find it in another handle maybe
                var oldItem = string.IsNullOrWhiteSpace(item.Region) ? handle.GetCacheItem(item.Key) : handle.GetCacheItem(item.Key, item.Region);

                handle.Stats.OnPut(item, oldItem == null);
            }

            handle.Put(item);
        }

        // update backplane
        if (_cacheBackplane != null)
        {
            if (string.IsNullOrWhiteSpace(item.Region))
            {
                _cacheBackplane.NotifyChange(item.Key, CacheItemChangedEventAction.Put);
            }
            else
            {
                _cacheBackplane.NotifyChange(item.Key, item.Region, CacheItemChangedEventAction.Put);
            }
        }

        TriggerOnPut(item.Key, item.Region);
    }

    /// <inheritdoc />
    protected override void Dispose(bool disposeManaged)
    {
        if (disposeManaged)
        {
            foreach (var handle in _cacheHandles)
            {
                handle.Dispose();
            }

            _cacheBackplane?.Dispose();
        }

        base.Dispose(disposeManaged);
    }

    /// <inheritdoc />
    protected override CacheItem<TValue> GetCacheItemInternal(string key) =>
        GetCacheItemInternal(key, null);

    /// <inheritdoc />
    protected override CacheItem<TValue> GetCacheItemInternal(string key, string region)
    {
        CheckDisposed();

        CacheItem<TValue> cacheItem = null;

        for (var handleIndex = 0; handleIndex < _cacheHandles.Length; handleIndex++)
        {
            var handle = _cacheHandles[handleIndex];
            cacheItem = string.IsNullOrWhiteSpace(region) ? handle.GetCacheItem(key) : handle.GetCacheItem(key, region);

            handle.Stats.OnGet(region);

            if (cacheItem != null)
            {
                cacheItem.LastAccessedUtc = DateTime.UtcNow;

                // update other handles if needed
                AddToHandles(cacheItem, handleIndex);
                handle.Stats.OnHit(region);
                TriggerOnGet(key, region);
                break;
            }
            else
            {
                handle.Stats.OnMiss(region);
            }
        }

        return cacheItem;
    }

    /// <inheritdoc />
    protected override bool RemoveInternal(string key) =>
        RemoveInternal(key, null);

    /// <inheritdoc />
    protected override bool RemoveInternal(string key, string region)
    {
        CheckDisposed();

        var result = false;

        foreach (var handle in _cacheHandles)
        {
            var handleResult = !string.IsNullOrWhiteSpace(region) ? handle.Remove(key, region) : handle.Remove(key);

            if (!handleResult)
            {
                continue;
            }

            result = true;
            handle.Stats.OnRemove(region);
        }

        if (result)
        {
            // update backplane
            if (_cacheBackplane != null)
            {
                if (string.IsNullOrWhiteSpace(region))
                {
                    _cacheBackplane.NotifyRemove(key);
                }
                else
                {
                    _cacheBackplane.NotifyRemove(key, region);
                }
            }

            // trigger only once and not per handle
            TriggerOnRemove(key, region);
        }

        return result;
    }

    private static bool AddItemToHandle(CacheItem<TValue> item, BaseCacheHandle<TValue> handle)
    {
        if (handle.Add(item))
        {
            handle.Stats.OnAdd(item);
            return true;
        }

        return false;
    }

    private static void ClearHandles(IEnumerable<BaseCacheHandle<TValue>> handles)
    {
        foreach (var handle in handles)
        {
            handle.Clear();
            handle.Stats.OnClear();
        }

        ////this.TriggerOnClear();
    }

    private static void ClearRegionHandles(string region, IEnumerable<BaseCacheHandle<TValue>> handles)
    {
        foreach (var handle in handles)
        {
            handle.ClearRegion(region);
            handle.Stats.OnClearRegion(region);
        }

        ////this.TriggerOnClearRegion(region);
    }

    private static void EvictFromHandles(string key, string region, IEnumerable<BaseCacheHandle<TValue>> handles)
    {
        foreach (var handle in handles)
        {
            EvictFromHandle(key, region, handle);
        }
    }

    private static void EvictFromHandle(string key, string region, BaseCacheHandle<TValue> handle)
    {
        var result = string.IsNullOrWhiteSpace(region) ? handle.Remove(key) : handle.Remove(key, region);

        if (result)
        {
            handle.Stats.OnRemove(region);
        }
    }

    private void AddToHandles(CacheItem<TValue> item, int foundIndex)
    {
        if (foundIndex == 0)
        {
            return;
        }

        // update all cache handles with lower order, up the list
        for (var handleIndex = 0; handleIndex < _cacheHandles.Length; handleIndex++)
        {
            if (handleIndex < foundIndex)
            {
                _cacheHandles[handleIndex].Add(item);
            }
        }
    }

    private void AddToHandlesBelow(CacheItem<TValue> item, int foundIndex)
    {
        if (item == null)
        {
            return;
        }

        for (var handleIndex = 0; handleIndex < _cacheHandles.Length; handleIndex++)
        {
            if (handleIndex > foundIndex)
            {
                if (_cacheHandles[handleIndex].Add(item))
                {
                    _cacheHandles[handleIndex].Stats.OnAdd(item);
                }
            }
        }
    }

    private void EvictFromOtherHandles(string key, string region, int excludeIndex)
    {
        if (excludeIndex < 0 || excludeIndex >= _cacheHandles.Length)
        {
            throw new ArgumentOutOfRangeException(nameof(excludeIndex));
        }

        for (var handleIndex = 0; handleIndex < _cacheHandles.Length; handleIndex++)
        {
            if (handleIndex != excludeIndex)
            {
                EvictFromHandle(key, region, _cacheHandles[handleIndex]);
            }
        }
    }

    private void EvictFromHandlesAbove(string key, string region, int excludeIndex)
    {
        if (excludeIndex < 0 || excludeIndex >= _cacheHandles.Length)
        {
            throw new ArgumentOutOfRangeException(nameof(excludeIndex));
        }

        for (var handleIndex = 0; handleIndex < _cacheHandles.Length; handleIndex++)
        {
            if (handleIndex < excludeIndex)
            {
                EvictFromHandle(key, region, _cacheHandles[handleIndex]);
            }
        }
    }

    private void RegisterCacheBackplane(CacheBackplane backplane)
    {
        Check.EnsureNotNull(backplane, nameof(backplane));

        // this should have been checked during activation already, just to be totally sure...
        if (_cacheHandles.Any(p => p.Configuration.IsBackplaneSource))
        {
            // added includeSource param to get the handles which need to be synced.
            // in case the backplane source is non-distributed (in-memory), only remotely triggered remove and clear should also
            // trigger a sync locally. For distributed caches, we expect that the distributed cache is already the source and in sync
            // as that's the layer which triggered the event. In this case, only other in-memory handles above the distributed, would be synced.
            var handles = new Func<bool, BaseCacheHandle<TValue>[]>(includeSource =>
            {
                var handleList = new List<BaseCacheHandle<TValue>>();
                foreach (var handle in _cacheHandles)
                {
                    if (!handle.Configuration.IsBackplaneSource ||
                        (includeSource && handle.Configuration.IsBackplaneSource && !handle.IsDistributedCache))
                    {
                        handleList.Add(handle);
                    }
                }

                return handleList.ToArray();
            });

            backplane.Changed += (_, args) =>
            {
                EvictFromHandles(args.Key, args.Region, handles(false));
                switch (args.Action)
                {
                    case CacheItemChangedEventAction.Add:
                        TriggerOnAdd(args.Key, args.Region, CacheActionEventArgOrigin.Remote);
                        break;

                    case CacheItemChangedEventAction.Put:
                        TriggerOnPut(args.Key, args.Region, CacheActionEventArgOrigin.Remote);
                        break;

                    case CacheItemChangedEventAction.Update:
                        TriggerOnUpdate(args.Key, args.Region, CacheActionEventArgOrigin.Remote);
                        break;
                }
            };

            backplane.Removed += (_, args) =>
            {
                EvictFromHandles(args.Key, args.Region, handles(true));
                TriggerOnRemove(args.Key, args.Region, CacheActionEventArgOrigin.Remote);
            };

            backplane.Cleared += (_, _) =>
            {
                ClearHandles(handles(true));
                TriggerOnClear(CacheActionEventArgOrigin.Remote);
            };

            backplane.ClearedRegion += (_, args) =>
            {
                ClearRegionHandles(args.Region, handles(true));
                TriggerOnClearRegion(args.Region, CacheActionEventArgOrigin.Remote);
            };
        }
    }

    private void TriggerOnAdd(string key, string region, CacheActionEventArgOrigin origin = CacheActionEventArgOrigin.Local)
    {
        OnAdd?.Invoke(this, new CacheActionEventArgs(key, region, origin));
    }

    private void TriggerOnClear(CacheActionEventArgOrigin origin = CacheActionEventArgOrigin.Local)
    {
        OnClear?.Invoke(this, new CacheClearEventArgs(origin));
    }

    private void TriggerOnClearRegion(string region, CacheActionEventArgOrigin origin = CacheActionEventArgOrigin.Local)
    {
        OnClearRegion?.Invoke(this, new CacheClearRegionEventArgs(region, origin));
    }

    private void TriggerOnGet(string key, string region, CacheActionEventArgOrigin origin = CacheActionEventArgOrigin.Local)
    {
        OnGet?.Invoke(this, new CacheActionEventArgs(key, region, origin));
    }

    private void TriggerOnPut(string key, string region, CacheActionEventArgOrigin origin = CacheActionEventArgOrigin.Local)
    {
        OnPut?.Invoke(this, new CacheActionEventArgs(key, region, origin));
    }

    private void TriggerOnRemove(string key, string region, CacheActionEventArgOrigin origin = CacheActionEventArgOrigin.Local)
    {
        Check.EnsureNotNullOrWhiteSpace(key, nameof(key));
        OnRemove?.Invoke(this, new CacheActionEventArgs(key, region, origin));
    }

    private void TriggerOnRemoveByHandle(string key, string region, CacheItemRemovedReason reason, int level, object value)
    {
        Check.EnsureNotNullOrWhiteSpace(key, nameof(key));
        OnRemoveByHandle?.Invoke(this, new CacheItemRemovedEventArgs(key, region, reason, value, level));
    }

    private void TriggerOnUpdate(string key, string region, CacheActionEventArgOrigin origin = CacheActionEventArgOrigin.Local)
    {
        OnUpdate?.Invoke(this, new CacheActionEventArgs(key, region, origin));
    }
}