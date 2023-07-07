using Nerosoft.Euonia.Collections;

namespace Nerosoft.Euonia.Threading.ZooKeeper;

internal sealed record ZooKeeperAuthInfo(string Scheme, EquatableReadOnlyList<byte> Auth);