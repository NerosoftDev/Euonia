using StackExchange.Redis;

namespace Nerosoft.Euonia.Threading.Redis;

internal class RedisMutexPrimitive : IRedisLockAcquirableSynchronizationPrimitive, IRedisLockExtensibleSynchronizationPrimitive
{
    private readonly RedisKey _key;
    private readonly RedisValue _lockId;
    private readonly RedisLockTimeouts _timeouts;

    public RedisMutexPrimitive(RedisKey key, RedisValue lockId, RedisLockTimeouts timeouts)
    {
        _key = key;
        _lockId = lockId;
        _timeouts = timeouts;
    }

    public TimeoutValue AcquireTimeout => _timeouts.AcquireTimeout;

    private static readonly RedisScript<RedisMutexPrimitive> ReleaseScript = new(@"
            if redis.call('get', @key) == @lockId then
                return redis.call('del', @key)
            end
            return 0",
        p => new { key = p._key, lockId = p._lockId }
    );

    public void Release(IDatabase database, bool fireAndForget) => ReleaseScript.Execute(database, this, fireAndForget);
    public Task ReleaseAsync(IDatabaseAsync database, bool fireAndForget) => ReleaseScript.ExecuteAsync(database, this, fireAndForget);

    public bool TryAcquire(IDatabase database) =>
        database.StringSet(_key, _lockId, _timeouts.Expiry.TimeSpan, When.NotExists, CommandFlags.DemandMaster);
    public Task<bool> TryAcquireAsync(IDatabaseAsync database) =>
        database.StringSetAsync(_key, _lockId, _timeouts.Expiry.TimeSpan, When.NotExists, CommandFlags.DemandMaster);

    private static readonly RedisScript<RedisMutexPrimitive> ExtendScript = new(@"
            if redis.call('get', @key) == @lockId then
                return redis.call('pexpire', @key, @expiryMillis)
            end
            return 0",
        p => new { key = p._key, lockId = p._lockId, expiryMillis = p._timeouts.Expiry.InMilliseconds }
    );

    public Task<bool> TryExtendAsync(IDatabaseAsync database) => ExtendScript.ExecuteAsync(database, this).AsBooleanTask();
}