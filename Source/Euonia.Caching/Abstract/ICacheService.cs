// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global

namespace Nerosoft.Euonia.Caching;

/// <summary>
/// The base service contract of Caching.
/// </summary>
public interface ICacheService
{
    /// <summary>
    /// Get cached value with specified key.
    /// </summary>
    /// <param name="key">Cache item key.</param>
    /// <typeparam name="TValue">Type of value.</typeparam>
    /// <returns></returns>
    TValue Get<TValue>(string key);

    /// <summary>
    /// Try get cached value with specified key.
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <typeparam name="TValue"></typeparam>
    /// <returns></returns>
    bool TryGet<TValue>(string key, out TValue value);

    /// <summary>
    /// Get exist value or add new while the specified key not exists.
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="key"></param>
    /// <param name="factory"></param>
    /// <param name="timeout"></param>
    /// <returns></returns>
    TValue GetOrAdd<TValue>(string key, Func<TValue> factory, TimeSpan? timeout = null);

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="key"></param>
    /// <param name="factory"></param>
    /// <param name="timeout"></param>
    /// <param name="isUtcTime"></param>
    /// <returns></returns>
    TValue GetOrAdd<TValue>(string key, Func<TValue> factory, DateTime timeout, bool isUtcTime = true);

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="key"></param>
    /// <param name="factory"></param>
    /// <param name="timeout"></param>
    /// <returns></returns>
    TValue AddOrUpdate<TValue>(string key, Func<TValue> factory, TimeSpan? timeout = null);

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="key"></param>
    /// <param name="factory"></param>
    /// <param name="timeout"></param>
    /// <param name="isUtcTime"></param>
    /// <returns></returns>
    TValue AddOrUpdate<TValue>(string key, Func<TValue> factory, DateTime timeout, bool isUtcTime = true);

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <param name="timeout"></param>
    /// <returns></returns>
    TValue AddOrUpdate<TValue>(string key, TValue value, TimeSpan? timeout = null);

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <param name="timeout"></param>
    /// <param name="isUtcTime"></param>
    /// <returns></returns>
    TValue AddOrUpdate<TValue>(string key, TValue value, DateTime timeout, bool isUtcTime = true);

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="item"></param>
    /// <returns></returns>
    TValue AddOrUpdate<TValue>(CacheItem<TValue> item);

    /// <summary>
    /// Remove cached value of specified key.
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="key"></param>
    /// <returns></returns>
    bool Remove<TValue>(string key);

    /// <summary>
    /// Get exist value or add new while the specified key not exists asynchronously.
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="key"></param>
    /// <param name="factory"></param>
    /// <param name="timeout"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<TValue> GetOrAddAsync<TValue>(string key, Func<Task<TValue>> factory, TimeSpan? timeout = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="key"></param>
    /// <param name="factory"></param>
    /// <param name="timeout"></param>
    /// <param name="isUtcTime"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<TValue> GetOrAddAsync<TValue>(string key, Func<Task<TValue>> factory, DateTime timeout, bool isUtcTime = true, CancellationToken cancellationToken = default);

    Task<TValue> AddOrUpdateAsync<TValue>(string key, Func<Task<TValue>> factory, TimeSpan? timeout = null, CancellationToken cancellationToken = default);

    Task<TValue> AddOrUpdateAsync<TValue>(string key, Func<Task<TValue>> factory, DateTime timeout, bool isUtcTime = true, CancellationToken cancellationToken = default);

    Task<TValue> AddOrUpdateAsync<TValue>(Func<Task<CacheItem<TValue>>> factory, CancellationToken cancellationToken = default);

    /// <summary>
    /// Generate cache key.
    /// </summary>
    /// <param name="separator"></param>
    /// <param name="values"></param>
    /// <returns></returns>
    string GenerateKey(string separator, params object[] values);
}