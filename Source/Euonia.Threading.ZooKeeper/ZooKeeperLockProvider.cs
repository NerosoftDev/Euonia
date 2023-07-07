﻿namespace Nerosoft.Euonia.Threading.ZooKeeper;

/// <summary>
/// An implementation of <see cref="ILockProvider"/> based on ZooKeeper. Uses the lock recipe described in
/// https://zookeeper.apache.org/doc/r3.1.2/recipes.html
/// </summary>
public sealed partial class ZooKeeperLockProvider : ILockProvider<ZooKeeperSynchronizationHandle>
{
    private readonly ZooKeeperSynchronizationHelper _synchronizationHelper;

    /// <summary>
    /// Constructs a new lock based on the provided <paramref name="path"/>, <paramref name="connectionString"/>, and <paramref name="options"/>.
    /// 
    /// If <paramref name="assumePathExists"/> is specified, then the node will not be created as part of acquiring nor will it be 
    /// deleted after releasing (defaults to false).
    /// </summary>
    public ZooKeeperLockProvider(
        ZooKeeperPath path,
        string connectionString,
        bool assumePathExists = false,
        Action<ZooKeeperSynchronizationOptionsBuilder> options = null)
        : this(path, assumePathExists: assumePathExists, connectionString, options)
    {
        if (path == default)
        {
            throw new ArgumentNullException(nameof(path));
        }

        if (path == ZooKeeperPath.Root)
        {
            throw new ArgumentException("Cannot be the root", nameof(path));
        }
    }

    /// <summary>
    /// Constructs a new lock based on the provided <paramref name="name"/>, <paramref name="connectionString"/>, and <paramref name="options"/>.
    /// 
    /// The lock's path will be a parent node of the root directory '/'. If <paramref name="name"/> is not a valid node name, it will be transformed to ensure
    /// validity.
    /// </summary>
    public ZooKeeperLockProvider(string name, string connectionString, Action<ZooKeeperSynchronizationOptionsBuilder> options = null)
        : this(ZooKeeperPath.Root, name, connectionString, options)
    {
    }

    /// <summary>
    /// Constructs a new lock based on the provided <paramref name="directoryPath"/>, <paramref name="name"/>, <paramref name="connectionString"/>, and <paramref name="options"/>.
    /// 
    /// The lock's path will be a parent node of <paramref name="directoryPath"/>. If <paramref name="name"/> is not a valid node name, it will be transformed to ensure
    /// validity.
    /// </summary>
    public ZooKeeperLockProvider(ZooKeeperPath directoryPath, string name, string connectionString, Action<ZooKeeperSynchronizationOptionsBuilder> options = null)
        : this(
            (directoryPath == default ? throw new ArgumentNullException(nameof(directoryPath)) : directoryPath).GetChildNodePathWithSafeName(name),
            assumePathExists: false,
            connectionString,
            options)
    {
    }

    private ZooKeeperLockProvider(ZooKeeperPath path, bool assumePathExists, string connectionString, Action<ZooKeeperSynchronizationOptionsBuilder> optionsBuilder) =>
        _synchronizationHelper = new ZooKeeperSynchronizationHelper(path, assumePathExists, connectionString, optionsBuilder);

    /// <summary>
    /// The zookeeper node path
    /// </summary>
    private ZooKeeperPath Path => _synchronizationHelper.Path;

    /// <summary>
    /// Implements <see cref="ILockProvider.Name"/>. Implemented explicitly to avoid confusion with the fact
    /// that this will include the leading "/" and base directory alongside the passed-in name.
    /// </summary>
    public string Name => Path.ToString();
}

public sealed partial class ZooKeeperLockProvider
{
    public ZooKeeperSynchronizationHandle TryAcquire(TimeSpan timeout = default, CancellationToken cancellationToken = default)
    {
        return Helpers.TryAcquire(this, timeout, cancellationToken);
    }

    public ZooKeeperSynchronizationHandle Acquire(TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        return Helpers.Acquire(this, timeout, cancellationToken);
    }

    public async ValueTask<ZooKeeperSynchronizationHandle> TryAcquireAsync(TimeoutValue timeout, CancellationToken cancellationToken)
    {
        var nodeHandle = await _synchronizationHelper.TryAcquireAsync(
                                                         hasAcquired: state => state.SortedChildren[0].Path == state.EphemeralNodePath,
                                                         waitAsync: async (zooKeeper, state, watcher) =>
                                                         {
                                                             var ephemeralNodeIndex = Array.FindIndex(state.SortedChildren, t => t.Path == state.EphemeralNodePath);
                                                             var nextLowestChildNode = state.SortedChildren[ephemeralNodeIndex - 1].Path;
                                                             // If the next lowest child node is already gone, then the wait is done. Otherwise, leave the watcher on that
                                                             // node so that we'll be notified when it changes (we can't acquire the lock before then)
                                                             return await zooKeeper.existsAsync(nextLowestChildNode, watcher).ConfigureAwait(false) == null;
                                                         },
                                                         timeout,
                                                         cancellationToken,
                                                         nodePrefix: "lock-"
                                                     )
                                                     // we're forced to use sync-over-async here because ZooKeeperNetEx doesn't have synchronous APIs
                                                     .AwaitSyncOverAsync()
                                                     .ConfigureAwait(false);

        return nodeHandle != null ? new ZooKeeperSynchronizationHandle(nodeHandle) : null;
    }

    public ValueTask<ZooKeeperSynchronizationHandle> TryAcquireAsync(TimeSpan timeout = default, CancellationToken cancellationToken = default)
    {
        return this.As<ILockProvider<ZooKeeperSynchronizationHandle>>().TryAcquireAsync(timeout, cancellationToken);
    }

    public ValueTask<ZooKeeperSynchronizationHandle> AcquireAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        return Helpers.AcquireAsync(this, timeout, cancellationToken);
    }
}

public sealed partial class ZooKeeperLockProvider
{
    ISynchronizationHandle ILockProvider.TryAcquire(TimeSpan timeout, CancellationToken cancellationToken)
    {
        return TryAcquire(timeout, cancellationToken);
    }

    ISynchronizationHandle ILockProvider.Acquire(TimeSpan? timeout, CancellationToken cancellationToken)
    {
        return Acquire(timeout, cancellationToken);
    }

    ValueTask<ISynchronizationHandle> ILockProvider.TryAcquireAsync(TimeSpan timeout, CancellationToken cancellationToken)
    {
        return TryAcquireAsync(timeout, cancellationToken).Convert(TaskConversion<ISynchronizationHandle>.ValueTask);
    }

    ValueTask<ISynchronizationHandle> ILockProvider.AcquireAsync(TimeSpan? timeout, CancellationToken cancellationToken)
    {
        return AcquireAsync(timeout, cancellationToken).Convert(TaskConversion<ISynchronizationHandle>.ValueTask);
    }

    ValueTask<ISynchronizationHandle> ILockProvider.TryAcquireAsync(TimeoutValue timeout, CancellationToken cancellationToken)
    {
        return TryAcquireAsync(timeout, cancellationToken).Convert(TaskConversion<ISynchronizationHandle>.ValueTask);
    }
}