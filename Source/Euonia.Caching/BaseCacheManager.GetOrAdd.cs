namespace Nerosoft.Euonia.Caching;

public partial class BaseCacheManager<TValue>
{
    /// <inheritdoc />
    public TValue GetOrAdd(string key, TValue value)
        => GetOrAdd(key, _ => value);

    /// <inheritdoc />
    public TValue GetOrAdd(string key, string region, TValue value)
        => GetOrAdd(key, region, (_, _) => value);

    /// <inheritdoc />
    public TValue GetOrAdd(string key, Func<string, TValue> valueFactory)
    {
        Check.EnsureNotNullOrWhiteSpace(key, nameof(key));
        Check.EnsureNotNull(valueFactory, nameof(valueFactory));

        return GetOrAddInternal(key, null, (k, _) => new CacheItem<TValue>(k, valueFactory(k))).Value;
    }

    /// <inheritdoc />
    public TValue GetOrAdd(string key, string region, Func<string, string, TValue> valueFactory)
    {
        Check.EnsureNotNullOrWhiteSpace(key, nameof(key));
        Check.EnsureNotNullOrWhiteSpace(region, nameof(region));
        Check.EnsureNotNull(valueFactory, nameof(valueFactory));

        return GetOrAddInternal(key, region, (k, r) => new CacheItem<TValue>(k, r, valueFactory(k, r))).Value;
    }

    /// <inheritdoc />
    public CacheItem<TValue> GetOrAdd(string key, Func<string, CacheItem<TValue>> valueFactory)
    {
        Check.EnsureNotNullOrWhiteSpace(key, nameof(key));
        Check.EnsureNotNull(valueFactory, nameof(valueFactory));

        return GetOrAddInternal(key, null, (k, _) => valueFactory(k));
    }

    /// <inheritdoc />
    public CacheItem<TValue> GetOrAdd(string key, string region, Func<string, string, CacheItem<TValue>> valueFactory)
    {
        Check.EnsureNotNullOrWhiteSpace(key, nameof(key));
        Check.EnsureNotNullOrWhiteSpace(region, nameof(region));
        Check.EnsureNotNull(valueFactory, nameof(valueFactory));

        return GetOrAddInternal(key, region, valueFactory);
    }

    /// <inheritdoc />
    public bool TryGetOrAdd(string key, Func<string, TValue> valueFactory, out TValue value)
    {
        Check.EnsureNotNullOrWhiteSpace(key, nameof(key));
        Check.EnsureNotNull(valueFactory, nameof(valueFactory));

        if (TryGetOrAddInternal(
            key,
            null,
            (k, _) =>
            {
                var newValue = valueFactory(k);
                return newValue == null ? null : new CacheItem<TValue>(k, newValue);
            },
            out var item))
        {
            value = item.Value;
            return true;
        }

        value = default;
        return false;
    }

    /// <inheritdoc />
    public bool TryGetOrAdd(string key, string region, Func<string, string, TValue> valueFactory, out TValue value)
    {
        Check.EnsureNotNullOrWhiteSpace(key, nameof(key));
        Check.EnsureNotNullOrWhiteSpace(region, nameof(region));
        Check.EnsureNotNull(valueFactory, nameof(valueFactory));

        if (TryGetOrAddInternal(
            key,
            region,
            (k, r) =>
            {
                var newValue = valueFactory(k, r);
                return newValue == null ? null : new CacheItem<TValue>(k, r, newValue);
            },
            out var item))
        {
            value = item.Value;
            return true;
        }

        value = default;
        return false;
    }

    /// <inheritdoc />
    public bool TryGetOrAdd(string key, Func<string, CacheItem<TValue>> valueFactory, out CacheItem<TValue> item)
    {
        Check.EnsureNotNullOrWhiteSpace(key, nameof(key));
        Check.EnsureNotNull(valueFactory, nameof(valueFactory));

        return TryGetOrAddInternal(key, null, (k, _) => valueFactory(k), out item);
    }

    /// <inheritdoc />
    public bool TryGetOrAdd(string key, string region, Func<string, string, CacheItem<TValue>> valueFactory, out CacheItem<TValue> item)
    {
        Check.EnsureNotNullOrWhiteSpace(key, nameof(key));
        Check.EnsureNotNullOrWhiteSpace(region, nameof(region));
        Check.EnsureNotNull(valueFactory, nameof(valueFactory));

        return TryGetOrAddInternal(key, region, valueFactory, out item);
    }

    private bool TryGetOrAddInternal(string key, string region, Func<string, string, CacheItem<TValue>> valueFactory, out CacheItem<TValue> item)
    {
        CacheItem<TValue> newItem = null;
        var tries = 0;
        do
        {
            tries++;
            item = GetCacheItemInternal(key, region);
            if (item != null)
            {
                return true;
            }

            // changed logic to invoke the factory only once in case of retries
            newItem ??= valueFactory(key, region);

            if (newItem == null)
            {
                return false;
            }

            if (AddInternal(newItem))
            {
                item = newItem;
                return true;
            }
        }
        while (tries <= Configuration.MaxRetries);

        return false;
    }

    private CacheItem<TValue> GetOrAddInternal(string key, string region, Func<string, string, CacheItem<TValue>> valueFactory)
    {
        CacheItem<TValue> newItem = null;
        var tries = 0;
        do
        {
            tries++;
            var item = GetCacheItemInternal(key, region);
            if (item != null)
            {
                return item;
            }

            // changed logic to invoke the factory only once in case of retries
            newItem ??= valueFactory(key, region);

            // Throw explicit to me more consistent. Otherwise it would throw later eventually...
            if (newItem == null)
            {
                throw new InvalidOperationException("The CacheItem which should be added must not be null.");
            }

            if (AddInternal(newItem))
            {
                return newItem;
            }
        }
        while (tries <= Configuration.MaxRetries);

        // should usually never occur, but could if e.g. max retries is 1 and an item gets added between the get and add.
        // pretty unusual, so keep the max tries at least around 50
        throw new InvalidOperationException(
            string.Format("Could not get nor add the item {0} {1}", key, region));
    }
}
