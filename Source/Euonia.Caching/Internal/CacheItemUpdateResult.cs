namespace Nerosoft.Euonia.Caching.Internal;

/// <summary>
/// Helper class to create correct instances.
/// </summary>
public static class CacheItemUpdateResult
{
    /// <summary>
    /// Creates a new instance of the <see cref="CacheItemUpdateResult{TCacheValue}"/> class with
    /// properties typical for the case where the cache item did not exist for an update operation.
    /// </summary>
    /// <typeparam name="TValue">The type of the cache value.</typeparam>
    /// <returns>The item result.</returns>
    public static CacheItemUpdateResult<TValue> ForItemDidNotExist<TValue>() =>
        new(null, CacheItemUpdateResultState.ItemDidNotExist, false, 1);

    /// <summary>
    /// Creates a new instance of the <see cref="CacheItemUpdateResult{TCacheValue}"/> indicating that the
    /// cache value factory returned null instead of a valid value.
    /// </summary>
    /// <typeparam name="TCacheValue">The type of the cache value.</typeparam>
    /// <returns>The item result.</returns>
    public static CacheItemUpdateResult<TCacheValue> ForFactoryReturnedNull<TCacheValue>() =>
        new(null, CacheItemUpdateResultState.FactoryReturnedNull, false, 1);

    /// <summary>
    /// Creates a new instance of the <see cref="CacheItemUpdateResult{TCacheValue}"/> class with
    /// properties typical for a successful update operation.
    /// </summary>
    /// <typeparam name="TCacheValue">The type of the cache value.</typeparam>
    /// <param name="value">The value.</param>
    /// <param name="conflictOccurred">Set to <c>true</c> if a conflict occurred.</param>
    /// <param name="triesNeeded">The tries needed.</param>
    /// <returns>The item result.</returns>
    public static CacheItemUpdateResult<TCacheValue> ForSuccess<TCacheValue>(CacheItem<TCacheValue> value, bool conflictOccurred = false, int triesNeeded = 1) =>
        new(value, CacheItemUpdateResultState.Success, conflictOccurred, triesNeeded);

    /// <summary>
    /// Creates a new instance of the <see cref="CacheItemUpdateResult{TCacheValue}"/> class with
    /// properties typical for an update operation which failed because it exceeded the limit of tries.
    /// </summary>
    /// <typeparam name="TCacheValue">The type of the cache value.</typeparam>
    /// <param name="triesNeeded">The tries needed.</param>
    /// <returns>The item result.</returns>
    public static CacheItemUpdateResult<TCacheValue> ForTooManyRetries<TCacheValue>(int triesNeeded) =>
        new(null, CacheItemUpdateResultState.TooManyRetries, true, triesNeeded);
}

/// <summary>
/// Used by cache handle implementations to let the cache manager know what happened during an
/// update operation.
/// </summary>
/// <typeparam name="TValue">The type of the cache value.</typeparam>
public class CacheItemUpdateResult<TValue>
{
    internal CacheItemUpdateResult(CacheItem<TValue> value, CacheItemUpdateResultState state, bool conflictOccurred, int triesNeeded)
    {
        if (triesNeeded == 0)
        {
            throw new ArgumentOutOfRangeException(nameof(triesNeeded), "Value must be higher than 0.");
        }

        VersionConflictOccurred = conflictOccurred;
        UpdateState = state;
        NumberOfTriesNeeded = triesNeeded;
        Value = value;
    }

    /// <summary>
    /// Gets the number of tries the cache needed to update the item.
    /// </summary>
    /// <value>The number of retries needed.</value>
    public int NumberOfTriesNeeded { get; }

    /// <summary>
    /// Gets a value indicating whether the update operation was successful or not.
    /// </summary>
    /// <value>The current <see cref="CacheItemUpdateResultState"/>.</value>
    public CacheItemUpdateResultState UpdateState { get; }

    /// <summary>
    /// Gets the updated value.
    /// </summary>
    /// <value>The updated value.</value>
    public CacheItem<TValue> Value { get; }

    /// <summary>
    /// Gets a value indicating whether a version conflict occurred during an update operation.
    /// </summary>
    /// <value><c>true</c> if a version conflict occurred; otherwise, <c>false</c>.</value>
    public bool VersionConflictOccurred { get; }
}