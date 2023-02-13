namespace Nerosoft.Euonia.Caching;

/// <summary>
/// Provides services to enable parallel tasks aware of the current cache context.
/// </summary>
public interface IParallelCacheContext
{
    /// <summary>
    /// Create a task that wraps some piece of code that implictly depends on the cache context.
    /// The return task can be used in any execution thread (e.g. System.Threading.Tasks).
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="function">The function.</param>
    /// <returns>ITask&lt;T&gt;.</returns>
    CacheContextTask<T> CreateContextAwareTask<T>(Func<T> function);

    /// <summary>
    /// Runs the in parallel.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TResult">The type of the t result.</typeparam>
    /// <param name="source">The source.</param>
    /// <param name="selector">The selector.</param>
    /// <returns>IEnumerable&lt;TResult&gt;.</returns>
    IEnumerable<TResult> RunInParallel<T, TResult>(IEnumerable<T> source, Func<T, TResult> selector);
}