namespace Nerosoft.Euonia.Caching;

/// <summary>
/// The cache entry.
/// </summary>
public class CacheEntry
{
    /// <summary>
    /// Initialize a new instance.
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    public CacheEntry(string key, object value)
    {
        Key = key;
        Value = value;
    }

    /// <summary>
    /// Initialize a new instance.
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <param name="expirationOffset"></param>
    public CacheEntry(string key, object value, DateTimeOffset expirationOffset)
        : this(key, value)
    {
        ExpirationOffset = expirationOffset;
    }

    /// <summary>
    /// Initialize a new instance.
    /// </summary>
    /// <param name="key">The cache key.</param>
    /// <param name="value"></param>
    /// <param name="timeSpan"></param>
    public CacheEntry(string key, object value, TimeSpan timeSpan)
        : this(key, value)
    {
        ExpirationOffset = DateTime.UtcNow.Add(timeSpan);
    }

    /// <summary>
    /// The cache key.
    /// </summary>
    public string Key { get; set; }

    /// <summary>
    /// The cached value.
    /// </summary>
    public object Value { get; set; }

    /// <summary>
    /// The expired time.
    /// </summary>
    public DateTimeOffset? ExpirationOffset { get; set; }
}

/// <summary>
/// The cache entry with value of type <typeparamref name="TValue"/>.
/// </summary>
/// <typeparam name="TValue">The value type.</typeparam>
public class CacheEntry<TValue> : CacheEntry
{
    /// <summary>
    /// The cached value.
    /// </summary>
    public new TValue Value => (TValue)base.Value;

    /// <summary>
    /// Initialize a new instance.
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    public CacheEntry(string key, TValue value)
        : base(key, value)
    {
    }

    /// <summary>
    /// Initialize a new instance.
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <param name="expirationOffset"></param>
    public CacheEntry(string key, TValue value, DateTime expirationOffset)
        : base(key, value, expirationOffset)
    {
    }

    /// <summary>
    /// Initialize a new instance.
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <param name="timeSpan"></param>
    public CacheEntry(string key, TValue value, TimeSpan timeSpan)
        : base(key, value, timeSpan)
    {
    }
}