namespace Nerosoft.Euonia.Caching.Internal;

/// <summary>
/// Represents that state of an update operation.
/// </summary>
public enum CacheItemUpdateResultState
{
    /// <summary>
    /// The state represents a successful update operation.
    /// </summary>
    Success,

    /// <summary>
    /// The state represents a failed attempt. The retries limit had been reached.
    /// </summary>
    TooManyRetries,

    /// <summary>
    /// The state represents a failed attempt. The cache item did not exist, so no update could
    /// be made.
    /// </summary>
    ItemDidNotExist,

    /// <summary>
    /// Internal state representing a failure where the factory function returns null instead of a valid value.
    /// </summary>
    FactoryReturnedNull,
}
