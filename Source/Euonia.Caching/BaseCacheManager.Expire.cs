namespace Nerosoft.Euonia.Caching;

public partial class BaseCacheManager<TValue>
{
    /// <inheritdoc />
    public void Expire(string key, CacheExpirationMode mode, TimeSpan timeout)
        => ExpireInternal(key, null, mode, timeout);

    /// <inheritdoc />
    public void Expire(string key, string region, CacheExpirationMode mode, TimeSpan timeout)
        => ExpireInternal(key, region, mode, timeout);

    private void ExpireInternal(string key, string region, CacheExpirationMode mode, TimeSpan timeout)
    {
        CheckDisposed();

        var item = GetCacheItemInternal(key, region);
        if (item == null)
        {
            return;
        }

        if (mode == CacheExpirationMode.Absolute)
        {
            item = item.WithAbsoluteExpiration(timeout);
        }
        else if (mode == CacheExpirationMode.Sliding)
        {
            item = item.WithSlidingExpiration(timeout);
        }
        else if (mode == CacheExpirationMode.None)
        {
            item = item.WithNoExpiration();
        }
        else if (mode == CacheExpirationMode.Default)
        {
            item = item.WithDefaultExpiration();
        }

        PutInternal(item);
    }

    /// <inheritdoc />
    public void Expire(string key, DateTimeOffset absoluteExpiration)
    {
        var timeout = absoluteExpiration.UtcDateTime - DateTime.UtcNow;
        if (timeout <= TimeSpan.Zero)
        {
            throw new ArgumentException("Expiration value must be greater than zero.", nameof(absoluteExpiration));
        }

        Expire(key, CacheExpirationMode.Absolute, timeout);
    }

    /// <inheritdoc />
    public void Expire(string key, string region, DateTimeOffset absoluteExpiration)
    {
        var timeout = absoluteExpiration.UtcDateTime - DateTime.UtcNow;
        if (timeout <= TimeSpan.Zero)
        {
            throw new ArgumentException("Expiration value must be greater than zero.", nameof(absoluteExpiration));
        }

        Expire(key, region, CacheExpirationMode.Absolute, timeout);
    }

    /// <inheritdoc />
    public void Expire(string key, TimeSpan slidingExpiration)
    {
        if (slidingExpiration <= TimeSpan.Zero)
        {
            throw new ArgumentException("Expiration value must be greater than zero.", nameof(slidingExpiration));
        }

        Expire(key, CacheExpirationMode.Sliding, slidingExpiration);
    }

    /// <inheritdoc />
    public void Expire(string key, string region, TimeSpan slidingExpiration)
    {
        if (slidingExpiration <= TimeSpan.Zero)
        {
            throw new ArgumentException("Expiration value must be greater than zero.", nameof(slidingExpiration));
        }

        Expire(key, region, CacheExpirationMode.Sliding, slidingExpiration);
    }

    /// <inheritdoc />
    public void RemoveExpiration(string key)
    {
        Expire(key, CacheExpirationMode.None, default(TimeSpan));
    }

    /// <inheritdoc />
    public void RemoveExpiration(string key, string region)
    {
        Expire(key, region, CacheExpirationMode.None, default(TimeSpan));
    }
}
