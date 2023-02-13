namespace Nerosoft.Euonia.Caching;

/// <summary>
/// Interface ICacheManager
/// </summary>
public interface ICacheManager
{
    TResult Get<TKey, TResult>(TKey key);

    /// <summary>
    /// Gets the specified key.
    /// </summary>
    /// <typeparam name="TKey">The type of the t key.</typeparam>
    /// <typeparam name="TResult">The type of the t result.</typeparam>
    /// <param name="key">The key.</param>
    /// <param name="acquire">The acquire.</param>
    /// <returns>TResult.</returns>
    TResult GetOrAdd<TKey, TResult>(TKey key, Func<AcquireContext<TKey>, TResult> acquire);

    TResult AddOrUpdate<TKey, TResult>(TKey key, Func<AcquireContext<TKey>, TResult> acquire);

    /// <summary>
    /// Try gets the specified key.
    /// </summary>
    /// <typeparam name="TKey">The type of the t key.</typeparam>
    /// <typeparam name="TResult">The type of the t result.</typeparam>
    /// <param name="key">The key.</param>
    /// <param name="result">The cached value of key.</param>
    /// <returns></returns>
    bool TryGet<TKey, TResult>(TKey key, out TResult result);
}