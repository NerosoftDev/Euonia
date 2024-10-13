namespace Nerosoft.Euonia.Caching;

/// <summary>
/// Defines the possible update modes of the cache manager.
/// <para>
/// Update mode works on Get operations. If the cache manager finds the cache item in one of the
/// cache handles, and other cache handles do not have that item, it might add the item to the
/// other cache handles depending on the mode.
/// </para>
/// </summary>
public enum CacheUpdateMode
{
    /// <summary>
    /// Instructs the cache manager not to synchronize cache items with other cache handles (on <see cref="ICache{TCacheValue}.Get(string)"/> for example).
    /// </summary>
    None,
    
    /// <summary>
    /// Instructs the cache manager to synchronize cache items with other cache handles above in the list of cache handles.
    /// The order of cache handles is defined by the configuration.
    /// <remarks>
    /// This is the default behavior and should only be turned off if really needed.
    /// </remarks>
    /// </summary>
    Up
}