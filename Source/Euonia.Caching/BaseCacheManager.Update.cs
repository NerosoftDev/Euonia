using Nerosoft.Euonia.Caching.Internal;

namespace Nerosoft.Euonia.Caching;

public partial class BaseCacheManager<TValue>
{
    /// <inheritdoc />
    public TValue AddOrUpdate(string key, TValue addValue, Func<TValue, TValue> updateValue) =>
        AddOrUpdate(key, addValue, updateValue, Configuration.MaxRetries);

    /// <inheritdoc />
    public TValue AddOrUpdate(string key, string region, TValue addValue, Func<TValue, TValue> updateValue) =>
        AddOrUpdate(key, region, addValue, updateValue, Configuration.MaxRetries);

    /// <inheritdoc />
    public TValue AddOrUpdate(string key, TValue addValue, Func<TValue, TValue> updateValue, int maxRetries) =>
        AddOrUpdate(new CacheItem<TValue>(key, addValue), updateValue, maxRetries);

    /// <inheritdoc />
    public TValue AddOrUpdate(string key, string region, TValue addValue, Func<TValue, TValue> updateValue, int maxRetries) =>
        AddOrUpdate(new CacheItem<TValue>(key, region, addValue), updateValue, maxRetries);

    /// <inheritdoc />
    public TValue AddOrUpdate(CacheItem<TValue> addItem, Func<TValue, TValue> updateValue) =>
        AddOrUpdate(addItem, updateValue, Configuration.MaxRetries);

    /// <inheritdoc />
    public TValue AddOrUpdate(CacheItem<TValue> addItem, Func<TValue, TValue> updateValue, int maxRetries)
    {
        Check.EnsureNotNull(addItem, nameof(addItem));
        Check.EnsureNotNull(updateValue, nameof(updateValue));
        Check.Ensure(maxRetries >= 0, "Maximum number of retries must be greater than or equal to zero.");

        return AddOrUpdateInternal(addItem, updateValue, maxRetries);
    }

    private TValue AddOrUpdateInternal(CacheItem<TValue> item, Func<TValue, TValue> updateValue, int maxRetries)
    {
        CheckDisposed();

        var tries = 0;
        do
        {
            tries++;

            if (AddInternal(item))
            {
                return item.Value;
            }

            TValue returnValue;
            var updated = string.IsNullOrWhiteSpace(item.Region) ?
                TryUpdate(item.Key, updateValue, maxRetries, out returnValue) :
                TryUpdate(item.Key, item.Region, updateValue, maxRetries, out returnValue);

            if (updated)
            {
                return returnValue;
            }
        }
        while (tries <= maxRetries);

        // exceeded max retries, failing the operation... (should not happen in 99,99% of the cases though, better throw?)
        return default(TValue);
    }

    /// <inheritdoc />
    public bool TryUpdate(string key, Func<TValue, TValue> updateValue, out TValue value) =>
        TryUpdate(key, updateValue, Configuration.MaxRetries, out value);

    /// <inheritdoc />
    public bool TryUpdate(string key, string region, Func<TValue, TValue> updateValue, out TValue value) =>
        TryUpdate(key, region, updateValue, Configuration.MaxRetries, out value);

    /// <inheritdoc />
    public bool TryUpdate(string key, Func<TValue, TValue> updateValue, int maxRetries, out TValue value)
    {
        Check.EnsureNotNullOrWhiteSpace(key, nameof(key));
        Check.EnsureNotNull(updateValue, nameof(updateValue));
        Check.Ensure(maxRetries >= 0, "Maximum number of retries must be greater than or equal to zero.");

        return UpdateInternal(_cacheHandles, key, updateValue, maxRetries, false, out value);
    }

    /// <inheritdoc />
    public bool TryUpdate(string key, string region, Func<TValue, TValue> updateValue, int maxRetries, out TValue value)
    {
        Check.EnsureNotNullOrWhiteSpace(key, nameof(key));
        Check.EnsureNotNullOrWhiteSpace(region, nameof(region));
        Check.EnsureNotNull(updateValue, nameof(updateValue));
        Check.Ensure(maxRetries >= 0, "Maximum number of retries must be greater than or equal to zero.");

        return UpdateInternal(_cacheHandles, key, region, updateValue, maxRetries, false, out value);
    }

    /// <inheritdoc />
    public TValue Update(string key, Func<TValue, TValue> updateValue) =>
        Update(key, updateValue, Configuration.MaxRetries);

    /// <inheritdoc />
    public TValue Update(string key, string region, Func<TValue, TValue> updateValue) =>
        Update(key, region, updateValue, Configuration.MaxRetries);

    /// <inheritdoc />
    public TValue Update(string key, Func<TValue, TValue> updateValue, int maxRetries)
    {
        Check.EnsureNotNullOrWhiteSpace(key, nameof(key));
        Check.EnsureNotNull(updateValue, nameof(updateValue));
        Check.Ensure(maxRetries >= 0, "Maximum number of retries must be greater than or equal to zero.");

        var value = default(TValue);
        UpdateInternal(_cacheHandles, key, updateValue, maxRetries, true, out value);

        return value;
    }

    /// <inheritdoc />
    public TValue Update(string key, string region, Func<TValue, TValue> updateValue, int maxRetries)
    {
        Check.EnsureNotNullOrWhiteSpace(key, nameof(key));
        Check.EnsureNotNullOrWhiteSpace(region, nameof(region));
        Check.EnsureNotNull(updateValue, nameof(updateValue));
        Check.Ensure(maxRetries >= 0, "Maximum number of retries must be greater than or equal to zero.");

        var value = default(TValue);
        UpdateInternal(_cacheHandles, key, region, updateValue, maxRetries, true, out value);

        return value;
    }

    private bool UpdateInternal(BaseCacheHandle<TValue>[] handles,
        string key,
        Func<TValue, TValue> updateValue,
        int maxRetries,
        bool throwOnFailure,
        out TValue value) =>
        UpdateInternal(handles, key, null, updateValue, maxRetries, throwOnFailure, out value);

    private bool UpdateInternal(
        BaseCacheHandle<TValue>[] handles,
        string key,
        string region,
        Func<TValue, TValue> updateValue,
        int maxRetries,
        bool throwOnFailure,
        out TValue value)
    {
        CheckDisposed();

        // assign null
        value = default(TValue);

        if (handles.Length == 0)
        {
            return false;
        }

        // lowest level
        // todo: maybe check for only run on the backplate if configured (could potentially be not the last one).
        var handleIndex = handles.Length - 1;
        var handle = handles[handleIndex];

        var result = string.IsNullOrWhiteSpace(region) ?
            handle.Update(key, updateValue, maxRetries) :
            handle.Update(key, region, updateValue, maxRetries);

        if (result.UpdateState == CacheItemUpdateResultState.Success)
        {
            // only on success, the returned value will not be null
            value = result.Value.Value;
            handle.Stats.OnUpdate(key, region, result);

            // evict others, we don't know if the update on other handles could actually
            // succeed... There is a risk the update on other handles could create a
            // different version than we created with the first successful update... we can
            // safely add the item to handles below us though.
            EvictFromHandlesAbove(key, region, handleIndex);

            // optimizing - not getting the item again from cache. We already have it
            // var item = string.IsNullOrWhiteSpace(region) ? handle.GetCacheItem(key) : handle.GetCacheItem(key, region);
            AddToHandlesBelow(result.Value, handleIndex);
            TriggerOnUpdate(key, region);
        }
        else if (result.UpdateState == CacheItemUpdateResultState.FactoryReturnedNull)
        {
            if (throwOnFailure)
            {
                throw new InvalidOperationException($"Update failed on '{region}:{key}' because value factory returned null.");
            }
        }
        else if (result.UpdateState == CacheItemUpdateResultState.TooManyRetries)
        {
            // if we had too many retries, this basically indicates an
            // invalid state of the cache: The item is there, but we couldn't update it and
            // it most likely has a different version
            EvictFromOtherHandles(key, region, handleIndex);

            if (throwOnFailure)
            {
                throw new InvalidOperationException($"Update failed on '{region}:{key}' because of too many retries: {result.NumberOfTriesNeeded}.");
            }
        }
        else if (result.UpdateState == CacheItemUpdateResultState.ItemDidNotExist)
        {
            // If update fails because item doesn't exist AND the current handle is backplane source or the lowest cache handle level,
            // remove the item from other handles (if exists).
            // Otherwise, if we do not exit here, calling update on the next handle might succeed and would return a misleading result.
            EvictFromOtherHandles(key, region, handleIndex);

            if (throwOnFailure)
            {
                throw new InvalidOperationException($"Update failed on '{region}:{key}' because the region/key did not exist.");
            }
        }

        // update backplane
        if (result.UpdateState == CacheItemUpdateResultState.Success && _cacheBackplane != null)
        {
            if (string.IsNullOrWhiteSpace(region))
            {
                _cacheBackplane.NotifyChange(key, CacheItemChangedEventAction.Update);
            }
            else
            {
                _cacheBackplane.NotifyChange(key, region, CacheItemChangedEventAction.Update);
            }
        }

        return result.UpdateState == CacheItemUpdateResultState.Success;
    }
}
