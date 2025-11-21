using System.Diagnostics;

namespace Nerosoft.Euonia.Threading;

/// <summary>
/// Monitors the state of a lease and signals when it is lost.
/// </summary>
internal sealed class LeaseMonitor : IDisposable, IAsyncDisposable
{
    private readonly CancellationTokenSource _disposalSource = new(), _handleLostSource = new();

    private readonly ILeaseHandle _leaseHandle;
    private readonly Task _monitoringTask;
    private Task _cancellationTask;

    public LeaseMonitor(ILeaseHandle leaseHandle)
    {
        Invariant.Require(leaseHandle.LeaseDuration.CompareTo(leaseHandle.MonitoringCadence) >= 0);

        _leaseHandle = leaseHandle;
        _monitoringTask = CreateMonitoringLoopTask(new WeakReference<LeaseMonitor>(this), leaseHandle.MonitoringCadence, _disposalSource.Token);
    }

    public CancellationToken HandleLostToken => _handleLostSource.Token;

    public void Dispose() => this.DisposeSyncViaAsync();

    public async ValueTask DisposeAsync()
    {
        try
        {
            if (!_disposalSource.IsCancellationRequested) // idempotent
            {
                _disposalSource.Cancel();
            }

            await _monitoringTask.AwaitSyncOverAsync().ConfigureAwait(false);
        }
        finally
        {
            if (_cancellationTask != null)
            {
                _ = _cancellationTask.ContinueWith((_, state) => ((CancellationTokenSource)state).Dispose(), state: _handleLostSource, cancellationToken: HandleLostToken);
            }
            else
            {
                _handleLostSource.Dispose();
            }

            _disposalSource.Dispose();
        }
    }

    private static Task CreateMonitoringLoopTask(WeakReference<LeaseMonitor> weakMonitor, TimeoutValue monitoringCadence, CancellationToken disposalToken)
    {
        return Task.Run(MonitoringLoop, disposalToken);

        async Task MonitoringLoop()
        {
            var leaseLifetime = Stopwatch.StartNew();
            do
            {
                // wait until the next monitoring check
                await Task.Delay(monitoringCadence.InMilliseconds, disposalToken).TryAwait();
            }
            while (!disposalToken.IsCancellationRequested && await RunMonitoringLoopIterationAsync(weakMonitor, leaseLifetime).ConfigureAwait(false));
        }
    }

    private static async Task<bool> RunMonitoringLoopIterationAsync(WeakReference<LeaseMonitor> weakMonitor, Stopwatch leaseLifetime)
    {
        // if the monitor has been GC'd, just exit
        if (!weakMonitor.TryGetTarget(out var monitor))
        {
            return false;
        }

        // lease expired
        if (monitor._leaseHandle.LeaseDuration.CompareTo(leaseLifetime.Elapsed) < 0)
        {
            OnHandleLost();
            return false;
        }

        var leaseState = await monitor.CheckLeaseAsync().ConfigureAwait(false);
        switch (leaseState)
        {
            case LeaseState.Lost:
                OnHandleLost();
                return false;

            case LeaseState.Renewed:
                leaseLifetime.Restart();
                return true;

            // If the lease is held but not renewed or if we don't know (e. g. due to transient failure),
            // then just continue. We can't yet say that it is lost but it isn't renewed so we can't reset
            // the lifetime either.
            case LeaseState.Held:
            case LeaseState.Unknown:
                return true;

            default:
                throw new InvalidOperationException("should never get here");
        }

        // offload cancel to a background thread to avoid hangs or errors
        void OnHandleLost() => monitor._cancellationTask = Task.Run(() => monitor._handleLostSource.Cancel());
    }

    private async Task<LeaseState> CheckLeaseAsync()
    {
        var renewOrValidateTask = Helpers.SafeCreateTask(state => state.leaseHandle.RenewOrValidateLeaseAsync(state.Token), (leaseHandle: _leaseHandle, _disposalSource.Token));
        await renewOrValidateTask.TryAwait();
        return _disposalSource.IsCancellationRequested || renewOrValidateTask.Status != TaskStatus.RanToCompletion
            ? LeaseState.Unknown
            : renewOrValidateTask.Result;
    }

    public interface ILeaseHandle
    {
        TimeoutValue LeaseDuration { get; }
        TimeoutValue MonitoringCadence { get; }
        Task<LeaseState> RenewOrValidateLeaseAsync(CancellationToken cancellationToken);
    }

    public enum LeaseState
    {
        /// <summary>
        /// The lease is known to be still held but was not renewed
        /// </summary>
        Held,

        /// <summary>
        /// The lease has been renewed for <see cref="ILeaseHandle.LeaseDuration"/>
        /// </summary>
        Renewed,

        /// <summary>
        /// The lease is known to no longer be held
        /// </summary>
        Lost,

        /// <summary>
        /// The lease may or may not be held any longer
        /// </summary>
        Unknown,
    }
}