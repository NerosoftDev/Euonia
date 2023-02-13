namespace Nerosoft.Euonia.Caching;

/// <summary>
/// Class DefaultCacheContextAccessor.
/// Implements the <see cref="ICacheContextAccessor" />
/// </summary>
/// <seealso cref="ICacheContextAccessor" />
public class DefaultCacheContextAccessor : ICacheContextAccessor
{
    /// <summary>
    /// The thread instance
    /// </summary>
    [ThreadStatic]
    private static IAcquireContext _threadInstance;

    /// <summary>
    /// Gets or sets the thread instance.
    /// </summary>
    /// <value>The thread instance.</value>
    public static IAcquireContext ThreadInstance
    {
        get => _threadInstance;
        set => _threadInstance = value;
    }

    /// <inheritdoc />
    public IAcquireContext Current
    {
        get => ThreadInstance;
        set => ThreadInstance = value;
    }
}