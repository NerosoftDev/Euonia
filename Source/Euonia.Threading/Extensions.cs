namespace Nerosoft.Euonia.Threading;

public static class Extensions
{
    #region ILockFactory

    /// <summary>
    /// Equivalent to calling <see cref="ILockFactory.Create" /> and then
    /// <see cref="ILockProvider.TryAcquire(TimeSpan, CancellationToken)" />.
    /// </summary>
    public static ISynchronizationHandle TryAcquireLock(this ILockFactory provider, string name, TimeSpan timeout = default, CancellationToken cancellationToken = default) =>
        (provider ?? throw new ArgumentNullException(nameof(provider))).Create(name).TryAcquire(timeout, cancellationToken);

    /// <summary>
    /// Equivalent to calling <see cref="ILockFactory.Create" /> and then
    /// <see cref="ILockProvider.Acquire(TimeSpan?, CancellationToken)" />.
    /// </summary>
    public static ISynchronizationHandle AcquireLock(this ILockFactory provider, string name, TimeSpan? timeout = null, CancellationToken cancellationToken = default) =>
        (provider ?? throw new ArgumentNullException(nameof(provider))).Create(name).Acquire(timeout, cancellationToken);

    /// <summary>
    /// Equivalent to calling <see cref="ILockFactory.Create" /> and then
    /// <see cref="ILockProvider.TryAcquireAsync(TimeSpan, CancellationToken)" />.
    /// </summary>
    public static ValueTask<ISynchronizationHandle> TryAcquireLockAsync(this ILockFactory provider, string name, TimeSpan timeout = default, CancellationToken cancellationToken = default) =>
        (provider ?? throw new ArgumentNullException(nameof(provider))).Create(name).TryAcquireAsync(timeout, cancellationToken);

    /// <summary>
    /// Equivalent to calling <see cref="ILockFactory.Create" /> and then
    /// <see cref="ILockProvider.AcquireAsync(TimeSpan?, CancellationToken)" />.
    /// </summary>
    public static ValueTask<ISynchronizationHandle> AcquireLockAsync(this ILockFactory provider, string name, TimeSpan? timeout = null, CancellationToken cancellationToken = default) =>
        (provider ?? throw new ArgumentNullException(nameof(provider))).Create(name).AcquireAsync(timeout, cancellationToken);

    #endregion

    #region IDistributrdSemaphore

    /// <summary>
    /// Equivalent to calling <see cref="ISemaphoreFactory.Create" /> and then
    /// <see cref="ISemaphoreProvider.TryAcquire(TimeSpan, CancellationToken)" />.
    /// </summary>
    public static ISynchronizationHandle TryAcquireSemaphore(this ISemaphoreFactory provider, string name, int maxCount, TimeSpan timeout = default, CancellationToken cancellationToken = default) =>
        (provider ?? throw new ArgumentNullException(nameof(provider))).Create(name, maxCount).TryAcquire(timeout, cancellationToken);

    /// <summary>
    /// Equivalent to calling <see cref="ISemaphoreFactory.Create" /> and then
    /// <see cref="ISemaphoreProvider.Acquire(TimeSpan?, CancellationToken)" />.
    /// </summary>
    public static ISynchronizationHandle AcquireSemaphore(this ISemaphoreFactory provider, string name, int maxCount, TimeSpan? timeout = null, CancellationToken cancellationToken = default) =>
        (provider ?? throw new ArgumentNullException(nameof(provider))).Create(name, maxCount).Acquire(timeout, cancellationToken);

    /// <summary>
    /// Equivalent to calling <see cref="ISemaphoreFactory.Create" /> and then
    /// <see cref="ISemaphoreProvider.TryAcquireAsync(TimeSpan, CancellationToken)" />.
    /// </summary>
    public static ValueTask<ISynchronizationHandle> TryAcquireSemaphoreAsync(this ISemaphoreFactory provider, string name, int maxCount, TimeSpan timeout = default, CancellationToken cancellationToken = default) =>
        (provider ?? throw new ArgumentNullException(nameof(provider))).Create(name, maxCount).TryAcquireAsync(timeout, cancellationToken);

    /// <summary>
    /// Equivalent to calling <see cref="ISemaphoreFactory.Create" /> and then
    /// <see cref="ISemaphoreProvider.AcquireAsync(TimeSpan?, CancellationToken)" />.
    /// </summary>
    public static ValueTask<ISynchronizationHandle> AcquireSemaphoreAsync(this ISemaphoreFactory provider, string name, int maxCount, TimeSpan? timeout = null, CancellationToken cancellationToken = default) =>
        (provider ?? throw new ArgumentNullException(nameof(provider))).Create(name, maxCount).AcquireAsync(timeout, cancellationToken);

    #endregion
}