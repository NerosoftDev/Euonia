namespace Nerosoft.Euonia.Threading.Redis;

/// <summary>
/// Acts as a <see cref="Task.Delay(TimeSpan, CancellationToken)"/> which is cleaned up when
/// the <see cref="TimeoutTask"/> gets disposed
/// </summary>
public readonly struct TimeoutTask : IDisposable
{
    private readonly CancellationTokenSource _cleanupTokenSource;
    private readonly CancellationTokenSource _linkedTokenSource;

    /// <summary>
    /// Initialize a new instance of <see cref="TimeoutTask"/> with the specified timeout value.
    /// </summary>
    /// <param name="timeout"></param>
    /// <param name="cancellationToken"></param>
    public TimeoutTask(TimeoutValue timeout, CancellationToken cancellationToken)
    {
        _cleanupTokenSource = new CancellationTokenSource();
        _linkedTokenSource = cancellationToken.CanBeCanceled
            ? CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, _cleanupTokenSource.Token)
            : null;
        Task = Task.Delay(timeout.TimeSpan, _linkedTokenSource?.Token ?? _cleanupTokenSource.Token);
    }

    /// <summary>
    /// Gets the underlying <see cref="Task"/>
    /// </summary>
    public Task Task { get; }

    /// <inheritdoc />
    public void Dispose()
    {
        try
        {
            _cleanupTokenSource.Cancel();
        }
        finally
        {
            _linkedTokenSource?.Dispose();
            _cleanupTokenSource.Dispose();
        }
    }
}