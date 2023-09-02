using StackExchange.Redis;

namespace Nerosoft.Euonia.Threading.Redis;

/// <summary>
/// Implements a <see cref="ILockProvider"/> using Redis.
/// </summary>
public sealed partial class RedisLockProvider : ILockProvider<RedisSynchronizationHandle>
{
    private readonly IReadOnlyList<IDatabase> _databases;
    private readonly RedisSynchronizationOptions _options;

    /// <summary>
    /// Constructs a lock named <paramref name="key"/> using the provided <paramref name="database"/> and <paramref name="options"/>.
    /// </summary>
    public RedisLockProvider(RedisKey key, IDatabase database, Action<RedisSynchronizationOptionsBuilder> options = null)
        : this(key, new[] { database ?? throw new ArgumentNullException(nameof(database)) }, options)
    {
    }

    /// <summary>
    /// Constructs a lock named <paramref name="key"/> using the provided <paramref name="databases"/> and <paramref name="options"/>.
    /// </summary>
    public RedisLockProvider(RedisKey key, IEnumerable<IDatabase> databases, Action<RedisSynchronizationOptionsBuilder> options = null)
    {
        if (key == default(RedisKey))
        {
            throw new ArgumentNullException(nameof(key));
        }

        _databases = ValidateDatabases(databases);

        Key = key;
        _options = RedisSynchronizationOptionsBuilder.GetOptions(options);
    }

    internal static IReadOnlyList<IDatabase> ValidateDatabases(IEnumerable<IDatabase> databases)
    {
        var databasesArray = databases?.ToArray() ?? throw new ArgumentNullException(nameof(databases));
        if (databasesArray.Length == 0)
        {
            throw new ArgumentException("may not be empty", nameof(databases));
        }

        if (databasesArray.Contains(null!))
        {
            throw new ArgumentNullException(nameof(databases), "may not contain null");
        }

        return databasesArray;
    }

    /// <summary>
    /// The Redis key used to implement the lock
    /// </summary>
    private RedisKey Key { get; }

    /// <summary>
    /// Implements <see cref="ILockProvider.Name"/>
    /// </summary>
    public string Name => Key.ToString();
}

public sealed partial class RedisLockProvider
{
    /// <inheritdoc />
    public RedisSynchronizationHandle TryAcquire(TimeSpan timeout = default, CancellationToken cancellationToken = default)
    {
        return Helpers.TryAcquire(this, timeout, cancellationToken);
    }

    /// <inheritdoc />
    public RedisSynchronizationHandle Acquire(TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        return Helpers.Acquire(this, timeout, cancellationToken);
    }

    /// <inheritdoc />
    public ValueTask<RedisSynchronizationHandle> TryAcquireAsync(TimeSpan timeout = default, CancellationToken cancellationToken = default)
    {
        return this.As<ILockProvider<RedisSynchronizationHandle>>().TryAcquireAsync(timeout, cancellationToken);
    }

    /// <inheritdoc />
    public ValueTask<RedisSynchronizationHandle> AcquireAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        return Helpers.AcquireAsync(this, timeout, cancellationToken);
    }

    /// <inheritdoc />
    public ValueTask<RedisSynchronizationHandle> TryAcquireAsync(TimeoutValue timeout, CancellationToken cancellationToken)
    {
        return BusyWaitHelper.WaitAsync(
            state: this,
            tryGetValue: (@this, token) => @this.TryAcquireAsync(token),
            timeout: timeout,
            minSleepTime: _options.MinBusyWaitSleepTime,
            maxSleepTime: _options.MaxBusyWaitSleepTime,
            cancellationToken: cancellationToken
        );
    }

    private async ValueTask<RedisSynchronizationHandle> TryAcquireAsync(CancellationToken cancellationToken)
    {
        var primitive = new RedisMutexPrimitive(Key, RedisLockHelper.CreateLockId(), _options.RedisLockTimeouts);
        var tryAcquireTasks = await new RedisLockAcquire(primitive, _databases, cancellationToken).TryAcquireAsync().ConfigureAwait(false);
        return tryAcquireTasks != null
            ? new RedisSynchronizationHandle(new RedisLockHandle(primitive, tryAcquireTasks, extensionCadence: _options.ExtensionCadence, expiry: _options.RedisLockTimeouts.Expiry))
            : null;
    }
}

public sealed partial class RedisLockProvider
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