using StackExchange.Redis;

namespace Nerosoft.Euonia.Threading.Redis;

internal interface IRedisLockAcquirableSynchronizationPrimitive : IRedisLockReleasableSynchronizationPrimitive
{
    TimeoutValue AcquireTimeout { get; }
    Task<bool> TryAcquireAsync(IDatabaseAsync database);
    bool TryAcquire(IDatabase database);
}