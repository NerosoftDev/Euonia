namespace Nerosoft.Euonia.Caching;

/// <summary>
/// Class DefaultParallelCacheContext.
/// Implements the <see cref="IParallelCacheContext" />
/// </summary>
/// <seealso cref="IParallelCacheContext" />
public class DefaultParallelCacheContext : IParallelCacheContext
{
    /// <summary>
    /// The cache context accessor
    /// </summary>
    private readonly ICacheContextAccessor _cacheContextAccessor;

    /// <summary>
    /// Initializes a new instance of the <see cref="DefaultParallelCacheContext"/> class.
    /// </summary>
    /// <param name="cacheContextAccessor">The cache context accessor.</param>
    public DefaultParallelCacheContext(ICacheContextAccessor cacheContextAccessor)
    {
        _cacheContextAccessor = cacheContextAccessor;
    }

    /// <summary>
    /// Allow disabling parallel behavior through HostComponents.config
    /// </summary>
    /// <value><c>true</c> if disabled; otherwise, <c>false</c>.</value>
    public bool Disabled { get; set; }

    /// <summary>
    /// Runs the in parallel.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TResult">The type of the t result.</typeparam>
    /// <param name="source">The source.</param>
    /// <param name="selector">The selector.</param>
    /// <returns>IEnumerable&lt;TResult&gt;.</returns>
    public IEnumerable<TResult> RunInParallel<T, TResult>(IEnumerable<T> source, Func<T, TResult> selector)
    {
        if (Disabled)
        {
            return source.Select(selector);
        }
        else
        {
            // Create tasks that capture the current thread context
            var tasks = source.Select(item => CreateContextAwareTask(() => selector(item))).ToList();

            // Run tasks in parallel and combine results immediately
            var result = tasks
                         .AsParallel() // prepare for parallel execution
                         .AsOrdered() // preserve initial enumeration order
                         .Select(task => task.Execute()) // prepare tasks to run in parallel
                         .ToArray(); // force evaluation

            // Forward tokens collected by tasks to the current context
            foreach (var task in tasks)
            {
                task.Finish();
            }

            return result;
        }
    }

    /// <summary>
    /// Create a task that wraps some piece of code that impliticly depends on the cache context.
    /// The return task can be used in any execution thread (e.g. System.Threading.Tasks).
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="function">The function.</param>
    /// <returns>ITask&lt;T&gt;.</returns>
    public CacheContextTask<T> CreateContextAwareTask<T>(Func<T> function)
    {
        return new CacheContextTask<T>(_cacheContextAccessor, function);
    }
}