using StackExchange.Redis;

namespace Nerosoft.Euonia.Threading.Redis;

internal interface IRedisLockReleasableSynchronizationPrimitive
{
    Task ReleaseAsync(IDatabaseAsync database, bool fireAndForget);
    void Release(IDatabase database, bool fireAndForget);
}