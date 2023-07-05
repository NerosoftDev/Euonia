using StackExchange.Redis;

namespace Nerosoft.Euonia.Threading.Redis;

internal interface IRedisLockExtensibleSynchronizationPrimitive : IRedisLockReleasableSynchronizationPrimitive
{
    TimeoutValue AcquireTimeout { get; }
    Task<bool> TryExtendAsync(IDatabaseAsync database);
}