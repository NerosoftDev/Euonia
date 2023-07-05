namespace Nerosoft.Euonia.Threading.Redis;

internal readonly struct RedisSynchronizationOptions
{
    public RedisSynchronizationOptions(
        RedisLockTimeouts redisLockTimeouts,
        TimeoutValue extensionCadence,
        TimeoutValue minBusyWaitSleepTime,
        TimeoutValue maxBusyWaitSleepTime)
    {
        RedisLockTimeouts = redisLockTimeouts;
        ExtensionCadence = extensionCadence;
        MinBusyWaitSleepTime = minBusyWaitSleepTime;
        MaxBusyWaitSleepTime = maxBusyWaitSleepTime;
    }

    public RedisLockTimeouts RedisLockTimeouts { get; }
    public TimeoutValue ExtensionCadence { get; }
    public TimeoutValue MinBusyWaitSleepTime { get; }
    public TimeoutValue MaxBusyWaitSleepTime { get; }
}