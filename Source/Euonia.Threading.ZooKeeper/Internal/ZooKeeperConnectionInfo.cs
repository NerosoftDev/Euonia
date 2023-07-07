using Nerosoft.Euonia.Collections;

namespace Nerosoft.Euonia.Threading.ZooKeeper;

internal sealed record ZooKeeperConnectionInfo(string ConnectionString, TimeoutValue ConnectTimeout, TimeoutValue SessionTimeout, EquatableReadOnlyList<ZooKeeperAuthInfo> AuthInfo);