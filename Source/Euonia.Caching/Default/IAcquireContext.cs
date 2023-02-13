namespace Nerosoft.Euonia.Caching;

/// <summary>
/// Interface IAcquireContext
/// </summary>
public interface IAcquireContext
{
    /// <summary>
    /// Gets the monitor.
    /// </summary>
    /// <value>The monitor.</value>
    Action<IVolatileToken> Monitor { get; }
}