using Microsoft.Extensions.Options;

namespace Nerosoft.Euonia.Caching.Runtime;

/// <summary>
/// The implement of <see cref="ICacheService"/> with <see cref="System.Runtime.Caching"/>.
/// </summary>
public class RuntimeCacheService : ICacheService
{
    private readonly RuntimeCacheManager _manager;
    private readonly string _prefix;

    /// <summary>
    /// Initializes a new instance of the <see cref="RuntimeCacheService"/> class.
    /// </summary>
    /// <param name="options"></param>
    public RuntimeCacheService(IOptions<RuntimeCacheOptions> options)
    {
        _manager = new RuntimeCacheManager(options.Value);
        _prefix = options.Value.KeyPrefix;
    }

    /// <inheritdoc />
    public TValue Get<TValue>(string key)
    {
        key = RewriteKey(key);
        return _manager.Instance<TValue>().Get(key);
    }

    /// <inheritdoc />
    public bool TryGet<TValue>(string key, out TValue value)
    {
        key = RewriteKey(key);
        var result = _manager.Instance<TValue>().TryGetOrAdd(key, _ => null, out var cache);
        value = result ? cache.Value : default;
        return result;
    }

    /// <inheritdoc />
    public TValue GetOrAdd<TValue>(string key, Func<TValue> factory, TimeSpan? timeout = null)
    {
        key = RewriteKey(key);
        var result = _manager.Instance<TValue>().GetOrAdd(key, _ =>
        {
            var value = factory();
            return GetCacheItem(key, value, timeout);
        });
        return result.Value;
    }

    /// <inheritdoc />
    public TValue GetOrAdd<TValue>(string key, Func<TValue> factory, DateTime timeout, bool isUtcTime = true)
    {
        var timespan = timeout - (isUtcTime ? DateTime.UtcNow : DateTime.Now);

        return GetOrAdd(key, factory, timespan);
    }

    /// <inheritdoc />
    public TValue AddOrUpdate<TValue>(string key, Func<TValue> factory, TimeSpan? timeout = null)
    {
        var value = factory();
        return AddOrUpdate(key, value, timeout);
    }

    /// <inheritdoc />
    public TValue AddOrUpdate<TValue>(string key, Func<TValue> factory, DateTime timeout, bool isUtcTime = true)
    {
        var timespan = timeout - (isUtcTime ? DateTime.UtcNow : DateTime.Now);
        return AddOrUpdate(key, factory, timespan);
    }

    /// <inheritdoc />
    public TValue AddOrUpdate<TValue>(string key, TValue value, TimeSpan? timeout = null)
    {
        key = RewriteKey(key);
        var cacheItem = GetCacheItem(key, value, timeout);
        return _manager.Instance<TValue>().AddOrUpdate(cacheItem, _ => value);
    }

    /// <inheritdoc />
    public TValue AddOrUpdate<TValue>(string key, TValue value, DateTime timeout, bool isUtcTime = true)
    {
        var timespan = timeout - (isUtcTime ? DateTime.UtcNow : DateTime.Now);

        return AddOrUpdate(key, value, timespan);
    }

    /// <inheritdoc />
    public TValue AddOrUpdate<TValue>(CacheItem<TValue> item)
    {
        RewriteKey(item.Key);

        return _manager.Instance<TValue>().AddOrUpdate(item, _ => item.Value);
    }

    /// <inheritdoc />
    public bool Remove<TValue>(string key)
    {
        key = RewriteKey(key);
        return _manager.Instance<TValue>().Remove(key);
    }

    /// <inheritdoc />
    public async Task<TValue> GetOrAddAsync<TValue>(string key, Func<Task<TValue>> factory, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        key = RewriteKey(key);
        if (TryGet<TValue>(key, out var value))
        {
            return value;
        }

        value = await factory();
        var result = _manager.Instance<TValue>().GetOrAdd(key, _ => GetCacheItem(key, value, timeout));
        return result.Value;
    }

    /// <inheritdoc />
    public async Task<TValue> GetOrAddAsync<TValue>(string key, Func<Task<TValue>> factory, DateTime timeout, bool isUtcTime = true, CancellationToken cancellationToken = default)
    {
        var timespan = timeout - (isUtcTime ? DateTime.UtcNow : DateTime.Now);
        return await GetOrAddAsync(key, factory, timespan, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<TValue> AddOrUpdateAsync<TValue>(string key, Func<Task<TValue>> factory, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        key = RewriteKey(key);
        var value = await factory();
        return AddOrUpdate(key, value, timeout);
    }

    /// <inheritdoc />
    public async Task<TValue> AddOrUpdateAsync<TValue>(string key, Func<Task<TValue>> factory, DateTime timeout, bool isUtcTime = true, CancellationToken cancellationToken = default)
    {
        var timespan = timeout - (isUtcTime ? DateTime.UtcNow : DateTime.Now);
        return await AddOrUpdateAsync(key, factory, timespan, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<TValue> AddOrUpdateAsync<TValue>(Func<Task<CacheItem<TValue>>> factory, CancellationToken cancellationToken = default)
    {
        var item = await factory();
        return AddOrUpdate(item);
    }

    /// <inheritdoc />
    public string GenerateKey(string separator, params object[] values)
    {
        return $"{_prefix}.Cache.{string.Join(separator, values)}";
    }

    private string RewriteKey(string key)
    {
        if (string.IsNullOrEmpty(_prefix))
        {
            return key;
        }

        return key.StartsWith($"{_prefix}.Cache.", StringComparison.OrdinalIgnoreCase) ? key : $"{_prefix}.Cache.{key}";
    }

    private static CacheItem<TValue> GetCacheItem<TValue>(string key, TValue value, TimeSpan? timeout)
    {
        CacheItem<TValue> item;
        if (timeout > TimeSpan.Zero)
        {
            item = new CacheItem<TValue>(key, value, CacheExpirationMode.Absolute, timeout.Value);
        }
        else
        {
            item = new CacheItem<TValue>(key, value);
        }

        return item;
    }
}