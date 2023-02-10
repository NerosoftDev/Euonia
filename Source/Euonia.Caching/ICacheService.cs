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
    /// Get exist value or add new while the specified key not exists.
    /// </summary>
    /// <param name="key"></param>
    /// <param name="factory"></param>
    /// <typeparam name="TValue"></typeparam>
    /// <returns></returns>
    TValue GetOrAdd<TValue>(string key, Func<TValue> factory);

    /// <summary>
    /// Try get cached value with specified key.
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <typeparam name="TValue"></typeparam>
    /// <returns></returns>
    bool TryGet<TValue>(string key, out TValue value);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="key"></param>
    /// <param name="factory"></param>
    /// <typeparam name="TValue"></typeparam>
    TValue Set<TValue>(string key, Func<TValue> factory);

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    TValue Set<TValue>(string key, TValue value);

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="key"></param>
    /// <param name="factory"></param>
    /// <param name="timeout"></param>
    /// <returns></returns>
    TValue Set<TValue>(string key, Func<TValue> factory, TimeSpan timeout);

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <param name="timeout"></param>
    /// <returns></returns>
    TValue Set<TValue>(string key, TValue value, TimeSpan timeout);

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="key"></param>
    /// <param name="factory"></param>
    /// <param name="timeout"></param>
    /// <param name="isUtcTime"></param>
    /// <returns></returns>
    TValue Set<TValue>(string key, Func<TValue> factory, DateTime timeout, bool isUtcTime = true);

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <param name="timeout"></param>
    /// <param name="isUtcTime"></param>
    /// <returns></returns>
    TValue Set<TValue>(string key, TValue value, DateTime timeout, bool isUtcTime = true);

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="cacheEntry"></param>
    /// <returns></returns>
    TValue Set<TValue>(CacheEntry<TValue> cacheEntry);

    /// <summary>
    /// Remove cached value of specified key.
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="key"></param>
    /// <returns></returns>
    bool Remove<TValue>(string key);

    /// <summary>
    /// Generate cache key.
    /// </summary>
    /// <param name="separator"></param>
    /// <param name="values"></param>
    /// <returns></returns>
    string GenerateKey(string separator, params object[] values);
}