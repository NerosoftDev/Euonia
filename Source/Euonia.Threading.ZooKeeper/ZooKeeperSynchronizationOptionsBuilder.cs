﻿using Nerosoft.Euonia.Collections;
using org.apache.zookeeper.data;

namespace Nerosoft.Euonia.Threading.ZooKeeper;

/// <summary>
/// Options for configuring ZooKeeper-based synchronization primitives
/// </summary>
public sealed class ZooKeeperSynchronizationOptionsBuilder
{
    private static readonly IReadOnlyList<ACL> _defaultAcl = new[] { ZooKeeperNodeCreator.PublicAcl };

    /// <summary>
    /// According to https://bowenli86.github.io/2016/09/15/distributed%20system/zookeeper/ZooKeeper-Sessions-and-Session-Management/, 
    /// timeout can be a minimum of 2x the tick time and a maximum of 20x the tick time. The default tick time is 2s, so this default
    /// is set to be high enough to require relatively few heartbeats but also low enough to support either a 2s or 1s tick time by default.
    /// </summary>
    private TimeoutValue _sessionTimeout = TimeSpan.FromSeconds(20);

    /// <summary>
    /// Default value (arbitrarily) matches the default connection timeout for SQL Server
    /// </summary>
    private TimeoutValue _connectTimeout = TimeSpan.FromSeconds(15);

    private readonly List<ZooKeeperAuthInfo> _authInfo = new();
    private readonly List<ACL> _acl = new();

    private ZooKeeperSynchronizationOptionsBuilder()
    {
    }

    /// <summary>
    /// Configures the <paramref name="sessionTimeout"/> for connections to ZooKeeper. Because the underlying ZooKeeper client periodically renews
    /// the session, this value generally will not impact behavior. Lower values mean that locks will be released more quickly following a crash
    /// of the lock-holding process, but also increase the risk that transient connection issues will result in a dropped lock.
    /// 
    /// Defaults to 20s.
    /// </summary>
    public ZooKeeperSynchronizationOptionsBuilder SessionTimeout(TimeSpan sessionTimeout)
    {
        var sessionTimeoutValue = new TimeoutValue(sessionTimeout);
        if (sessionTimeoutValue.IsZero)
        {
            throw new ArgumentOutOfRangeException(nameof(sessionTimeout), ThreadingResources.IDS_VALUE_MUST_BE_POSITIVE);
        }

        if (sessionTimeoutValue.IsInfinite)
        {
            throw new ArgumentOutOfRangeException(nameof(sessionTimeout), ThreadingResources.IDS_CAN_NOT_BE_INFINITE);
        }

        _sessionTimeout = sessionTimeoutValue;
        return this;
    }

    /// <summary>
    /// Configures how long to wait to establish a connection to ZooKeeper before failing with a <see cref="TimeoutException"/>.
    /// 
    /// Defaults to 15s.
    /// </summary>
    public ZooKeeperSynchronizationOptionsBuilder ConnectTimeout(TimeSpan connectTimeout)
    {
        _connectTimeout = new TimeoutValue(connectTimeout);
        return this;
    }

    /// <summary>
    /// Specifies authentication info to be added to the Zookeeper connection with <see cref="org.apache.zookeeper.ZooKeeper.addAuthInfo"/>. Each call
    /// to this method adds another entry to the list of auth info. See https://zookeeper.apache.org/doc/r3.5.4-beta/zookeeperProgrammers.html for more
    /// information on ZooKeeper auth.
    /// 
    /// By default, no auth info is added.
    /// </summary>
    public ZooKeeperSynchronizationOptionsBuilder AddAuthInfo(string scheme, IReadOnlyList<byte> auth)
    {
        _authInfo.Add(new ZooKeeperAuthInfo(
            scheme ?? throw new ArgumentNullException(nameof(scheme)),
            new EquatableReadOnlyList<byte>(auth ?? throw new ArgumentNullException(nameof(auth)))
        ));
        return this;
    }

    /// <summary>
    /// Configures the access control list (ACL) for any created ZooKeeper nodes. Each call to this method adds another entry to the access control
    /// list. See https://zookeeper.apache.org/doc/r3.5.4-beta/zookeeperProgrammers.html for more information on ZooKeeper ACLs.
    /// 
    /// If no ACL entries are specified, the ACL used will be a singleton list that grants all permissions to (world, anyone).
    /// </summary>
    public ZooKeeperSynchronizationOptionsBuilder AddAccessControl(string scheme, string id, int permissionFlags)
    {
        _acl.Add(new ACL(permissionFlags, new Id(scheme ?? throw new ArgumentNullException(nameof(scheme)), id ?? throw new ArgumentNullException(nameof(id)))));
        return this;
    }

    internal static (TimeoutValue SessionTimeout, TimeoutValue ConnectTimeout, EquatableReadOnlyList<ZooKeeperAuthInfo> AuthInfo, IReadOnlyList<ACL> Acl) GetOptions(Action<ZooKeeperSynchronizationOptionsBuilder> options)
    {
        var builder = new ZooKeeperSynchronizationOptionsBuilder();
        options?.Invoke(builder);
        return (
            SessionTimeout: builder._sessionTimeout,
            ConnectTimeout: builder._connectTimeout,
            AuthInfo: new EquatableReadOnlyList<ZooKeeperAuthInfo>(builder._authInfo),
            Acl: builder._acl.Count != 0 ? builder._acl.ToArray() : _defaultAcl
        );
    }
}