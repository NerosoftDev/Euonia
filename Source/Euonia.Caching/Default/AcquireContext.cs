namespace Nerosoft.Euonia.Caching;

/// <summary>
/// Class AcquireContext.
/// Implements the <see cref="IAcquireContext" />
/// </summary>
/// <typeparam name="TKey">The type of the t key.</typeparam>
/// <seealso cref="IAcquireContext" />
public class AcquireContext<TKey> : IAcquireContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AcquireContext{TKey}"/> class.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <param name="monitor">The monitor.</param>
    public AcquireContext(TKey key, Action<IVolatileToken> monitor)
    {
        Key = key;
        Monitor = monitor;
    }

    /// <summary>
    /// Gets the key.
    /// </summary>
    /// <value>The key.</value>
    public TKey Key { get; }

    /// <summary>
    /// Gets the monitor.
    /// </summary>
    /// <value>The monitor.</value>
    public Action<IVolatileToken> Monitor { get; }
}

/// <summary>
/// Simple implementation of "IAcquireContext" given a lambda
/// Implements the <see cref="IAcquireContext" />
/// </summary>
/// <seealso cref="IAcquireContext" />
public class SimpleAcquireContext : IAcquireContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SimpleAcquireContext"/> class.
    /// </summary>
    /// <param name="monitor">The monitor.</param>
    public SimpleAcquireContext(Action<IVolatileToken> monitor)
    {
        Monitor = monitor;
    }

    /// <summary>
    /// Gets the monitor.
    /// </summary>
    /// <value>The monitor.</value>
    public Action<IVolatileToken> Monitor { get; }
}