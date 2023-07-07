namespace Nerosoft.Euonia.Threading;

/// <summary>
/// A mutex synchronization primitive which can be used to coordinate access to a resource or critical region of code
/// across processes or systems. The scope and capabilities of the lock are dependent on the particular implementation
/// </summary>
public interface ILockProvider
{
    /// <summary>
    /// A name that uniquely identifies the lock
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Acquires the lock synchronously, failing with <see cref="TimeoutException"/> if the attempt times out.
    /// </summary>
    /// <param name="timeout">How long to wait before giving up on the acquisition attempt. Defaults to <see cref="Timeout.InfiniteTimeSpan"/></param>
    /// <param name="cancellationToken">Specifies a token by which the wait can be canceled</param>
    /// <returns>An <see cref="ISynchronizationHandle"/> which can be used to release the lock</returns>
    /// <example>
    /// <code>
    ///     using (myLock.Acquire(...))
    ///     {
    ///         /* we have the lock! */
    ///     }
    ///     // dispose releases the lock
    /// </code>
    /// </example>
    ISynchronizationHandle Acquire(TimeSpan? timeout = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Attempts to acquire the lock synchronously.
    /// </summary>
    /// <param name="timeout">How long to wait before giving up on the acquisition attempt. Defaults to 0</param>
    /// <param name="cancellationToken">Specifies a token by which the wait can be canceled</param>
    /// <returns>An <see cref="ISynchronizationHandle"/> which can be used to release the lock or null on failure</returns>
    /// <example>
    /// <code>
    ///     using (var handle = myLock.TryAcquire(...))
    ///     {
    ///         if (handle != null) { /* we have the lock! */ }
    ///     }
    ///     // dispose releases the lock if we took it
    /// </code>
    /// </example>
    ISynchronizationHandle TryAcquire(TimeSpan timeout = default, CancellationToken cancellationToken = default);

    /// <summary>
    /// Acquires the lock asynchronously, failing with <see cref="TimeoutException"/> if the attempt times out.
    /// </summary>
    /// <param name="timeout">How long to wait before giving up on the acquisition attempt. Defaults to <see cref="Timeout.InfiniteTimeSpan"/></param>
    /// <param name="cancellationToken">Specifies a token by which the wait can be canceled</param>
    /// <returns>An <see cref="ISynchronizationHandle"/> which can be used to release the lock</returns>
    /// <example>
    /// <code>
    ///     await using (await myLock.AcquireAsync(...))
    ///     {
    ///         /* we have the lock! */
    ///     }
    ///     // dispose releases the lock
    /// </code>
    /// </example>
    ValueTask<ISynchronizationHandle> AcquireAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Attempts to acquire the lock asynchronously.
    /// </summary>
    /// <param name="timeout">How long to wait before giving up on the acquisition attempt. Defaults to 0</param>
    /// <param name="cancellationToken">Specifies a token by which the wait can be canceled</param>
    /// <returns>An <see cref="ISynchronizationHandle"/> which can be used to release the lock or null on failure</returns>
    /// <example>
    /// <code>
    ///     await using (var handle = await myLock.TryAcquireAsync(...))
    ///     {
    ///         if (handle != null) { /* we have the lock! */ }
    ///     }
    ///     // dispose releases the lock if we took it
    /// </code>
    /// </example>
    ValueTask<ISynchronizationHandle> TryAcquireAsync(TimeSpan timeout = default, CancellationToken cancellationToken = default);

    /// <summary>
    /// Attempts to acquire the lock asynchronously.
    /// </summary>
    /// <param name="timeout">How long to wait before giving up on the acquisition attempt. Defaults to 0</param>
    /// <param name="cancellationToken">Specifies a token by which the wait can be canceled</param>
    /// <returns>An <see cref="ISynchronizationHandle"/> which can be used to release the lock or null on failure</returns>
    /// <example>
    /// <code>
    ///     await using (var handle = await myLock.TryAcquireAsync(...))
    ///     {
    ///         if (handle != null) { /* we have the lock! */ }
    ///     }
    ///     // dispose releases the lock if we took it
    /// </code>
    /// </example>
    ValueTask<ISynchronizationHandle> TryAcquireAsync(TimeoutValue timeout, CancellationToken cancellationToken = default);
}

public interface ILockProvider<THandle> : ILockProvider
    where THandle : class, ISynchronizationHandle
{
    new THandle Acquire(TimeSpan? timeout = null, CancellationToken cancellationToken = default);
    new THandle TryAcquire(TimeSpan timeout = default, CancellationToken cancellationToken = default);
    new ValueTask<THandle> AcquireAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default);
    new ValueTask<THandle> TryAcquireAsync(TimeSpan timeout = default, CancellationToken cancellationToken = default);
    new ValueTask<THandle> TryAcquireAsync(TimeoutValue timeout, CancellationToken cancellationToken = default);
}