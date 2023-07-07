namespace Nerosoft.Euonia.Threading;

/// <summary>
/// Helper methods for working with tasks.
/// </summary>
public static class TaskHelper
{
    /// <summary>
    /// Executes a delegate synchronously, and captures its result in a task. The returned task is already completed.
    /// </summary>
    /// <param name="func">The delegate to execute synchronously.</param>
#pragma warning disable 1998
    public static async Task ExecuteAsTask(Action func)
#pragma warning restore 1998
    {
        func();
    }

    /// <summary>
    /// Executes a delegate synchronously, and captures its result in a task. The returned task is already completed.
    /// </summary>
    /// <param name="func">The delegate to execute synchronously.</param>
#pragma warning disable 1998
    public static async Task<T> ExecuteAsTask<T>(Func<T> func)
#pragma warning restore 1998
    {
        return func();
    }
    
    /// <summary>
    /// Gets a value indicating whether the current thread is running synchronously
    /// </summary>
    [field: ThreadStatic]
    public static bool IsSynchronous { get; private set; }

    /// <summary>
    /// Runs <paramref name="action"/> synchronously
    /// </summary>
    public static void Run<TState>(Func<TState, ValueTask> action, TState state)
    {
        Run(
            async s =>
            {
                await s.action(s.state).ConfigureAwait(false);
                return true;
            },
            (action, state)
        );
    }

    /// <summary>
    /// Runs <paramref name="action"/> synchronously
    /// </summary>
    public static TResult Run<TState, TResult>(Func<TState, ValueTask<TResult>> action, TState state)
    {
        Invariant.Require(!IsSynchronous);

        try
        {
            IsSynchronous = true;

            var task = action(state);
            Invariant.Require(task.IsCompleted);

            // this should never happen (and can't in the debug build). However, to make absolutely sure we have this as 
            // fallback logic for the release build
            if (!task.IsCompleted)
            {
                // call AsTask(), since https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.valuetask-1?view=netcore-3.1
                // says that we should not call GetAwaiter().GetResult() except on a completed ValueTask
                return task.AsTask().GetAwaiter().GetResult();
            }

            return task.GetAwaiter().GetResult();
        }
        finally
        {
            IsSynchronous = false;
        }
    }

    /// <summary>
    /// A <see cref="TaskHelper"/>-compatible implementation of <see cref="Task.Delay(TimeSpan, CancellationToken)"/>.
    /// </summary>
    public static ValueTask Delay(TimeoutValue timeout, CancellationToken cancellationToken)
    {
        if (!IsSynchronous)
        {
            return Task.Delay(timeout.InMilliseconds, cancellationToken).AsValueTask();
        }

        if (cancellationToken.CanBeCanceled)
        {
            if (cancellationToken.WaitHandle.WaitOne(timeout.InMilliseconds))
            {
                throw new OperationCanceledException("delay was canceled", cancellationToken);
            }
        }
        else
        {
            Thread.Sleep(timeout.InMilliseconds);
        }

        return default;
    }

    /// <summary>
    /// For a type <typeparamref name="TDisposable"/> which implements both <see cref="IAsyncDisposable"/> and <see cref="IDisposable"/>,
    /// provides an implementation of <see cref="IDisposable.Dispose"/> using <see cref="IAsyncDisposable.DisposeAsync"/>.
    /// </summary>
    public static void DisposeSyncViaAsync<TDisposable>(this TDisposable disposable)
        where TDisposable : IAsyncDisposable, IDisposable =>
        Run(@this => @this.DisposeAsync(), disposable);

    /// <summary>
    /// In synchronous mode, performs a blocking wait on the provided <paramref name="task"/>. In asynchronous mode,
    /// returns the <paramref name="task"/> as a <see cref="ValueTask{TResult}"/>.
    /// </summary>
    public static ValueTask<TResult> AwaitSyncOverAsync<TResult>(this Task<TResult> task) =>
        IsSynchronous ? task.GetAwaiter().GetResult().AsValueTask() : task.AsValueTask();

    /// <summary>
    /// In synchronous mode, performs a blocking wait on the provided <paramref name="task"/>. In asynchronous mode,
    /// returns the <paramref name="task"/> as a <see cref="ValueTask"/>.
    /// </summary>
    public static ValueTask AwaitSyncOverAsync(this Task task)
    {
        if (IsSynchronous) 
        { 
            task.GetAwaiter().GetResult();
            return default;
        }

        return task.AsValueTask();
    }
}