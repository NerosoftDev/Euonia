using System.Runtime.CompilerServices;
using System.Security.Cryptography;

namespace Nerosoft.Euonia.Threading;

internal static class Helpers
{
    /// <summary>
    /// Performs a type-safe cast
    /// </summary>
    public static T As<T>(this T @this) => @this;

    /// <summary>
    /// Performs a type-safe "cast" of a <see cref="ValueTask{TResult}"/>
    /// </summary>
    public static async ValueTask<TBase> Convert<TDerived, TBase>(this ValueTask<TDerived> task, TaskConversion<TBase>.ValueTaskConversion _)
        where TDerived : TBase
    {
        return await task.ConfigureAwait(false);
    }

    public static Task<TResult> SafeCreateTask<TState, TResult>(Func<TState, Task<TResult>> taskFactory, TState state) =>
        InternalSafeCreateTask<TState, Task<TResult>, TResult>(taskFactory, state);

    public static Task SafeCreateTask<TState>(Func<TState, Task> taskFactory, TState state) =>
        InternalSafeCreateTask<TState, Task, bool>(taskFactory, state);

    private static TTask InternalSafeCreateTask<TState, TTask, TResult>(Func<TState, TTask> taskFactory, TState state)
        where TTask : Task
    {
        try
        {
            return taskFactory(state);
        }
        catch (OperationCanceledException)
        {
            // don't use Task.FromCanceled here because oce.CancellationToken is not guaranteed to 
            // have IsCancellationRequested which FromCanceled requires
            var canceledTaskBuilder = new TaskCompletionSource<TResult>();
            canceledTaskBuilder.SetCanceled();
            return (TTask)canceledTaskBuilder.Task.As<object>();
        }
        catch (Exception ex)
        {
            return (TTask)Task.FromException<TResult>(ex).As<object>();
        }
    }

    public static ObjectDisposedException ObjectDisposed<T>(this T _) where T : IAsyncDisposable =>
        throw new ObjectDisposedException(typeof(T).ToString());

    public static NonThrowingAwaitable<TTask> TryAwait<TTask>(this TTask task) where TTask : Task => new(task);

    /// <summary>
    /// Throwing exceptions is slow and our workflow has us canceling tasks in the common case. Using this special awaitable
    /// allows for us to await those tasks without causing a thrown exception
    /// </summary>
    public readonly struct NonThrowingAwaitable<TTask> : ICriticalNotifyCompletion
        where TTask : Task
    {
        private readonly TTask _task;
        private readonly ConfiguredTaskAwaitable.ConfiguredTaskAwaiter _taskAwaiter;

        public NonThrowingAwaitable(TTask task)
        {
            _task = task;
            _taskAwaiter = task.ConfigureAwait(false).GetAwaiter();
        }

        public NonThrowingAwaitable<TTask> GetAwaiter() => this;

        public bool IsCompleted => _taskAwaiter.IsCompleted;

        public TTask GetResult()
        {
            // does NOT call _taskAwaiter.GetResult() since that could throw!

            Invariant.Require(_task.IsCompleted);
            return _task;
        }

        public void OnCompleted(Action continuation) => _taskAwaiter.OnCompleted(continuation);
        public void UnsafeOnCompleted(Action continuation) => _taskAwaiter.UnsafeOnCompleted(continuation);
    }

    public static bool TryGetValue<T>(this T? nullable, out T value)
        where T : struct
    {
        value = nullable.GetValueOrDefault();
        return nullable.HasValue;
    }

    public static string ToSafeName(string name, int maxNameLength, Func<string, string> convertToValidName)
    {
        if (name == null)
        {
            throw new ArgumentNullException(nameof(name));
        }

        var validBaseLockName = convertToValidName(name);
        if (validBaseLockName == name && validBaseLockName.Length <= maxNameLength)
        {
            return name;
        }

        using var sha = SHA512.Create();
        var hash = System.Convert.ToBase64String(sha.ComputeHash(Encoding.UTF8.GetBytes(name)));

        if (hash.Length >= maxNameLength)
        {
            return hash.Substring(0, length: maxNameLength);
        }

        var prefix = validBaseLockName.Substring(0, Math.Min(validBaseLockName.Length, maxNameLength - hash.Length));
        return prefix + hash;
    }

    public static async ValueTask<THandle> Wrap<THandle>(this ValueTask<ISynchronizationHandle> handleTask, Func<ISynchronizationHandle, THandle> factory)
        where THandle : class
    {
        var handle = await handleTask.ConfigureAwait(false);
        return handle != null ? factory(handle) : null;
    }

    #region ---- ILockProvider implementations ----

    public static ValueTask<THandle> AcquireAsync<THandle>(ILockProvider<THandle> @lock, TimeSpan? timeout, CancellationToken cancellationToken)
        where THandle : class, ISynchronizationHandle
    {
        return @lock.TryAcquireAsync(timeout, cancellationToken).ThrowTimeoutIfNull();
    }

    public static THandle Acquire<THandle>(ILockProvider<THandle> @lock, TimeSpan? timeout, CancellationToken cancellationToken)
        where THandle : class, ISynchronizationHandle
    {
        return TaskHelper.Run(
            state => AcquireAsync(state.@lock, state.timeout, state.cancellationToken),
            (@lock, timeout, cancellationToken)
        );
    }

    public static THandle TryAcquire<THandle>(ILockProvider<THandle> @lock, TimeSpan timeout, CancellationToken cancellationToken)
        where THandle : class, ISynchronizationHandle
    {
        return TaskHelper.Run(
            state => state.@lock.TryAcquireAsync(state.timeout, state.cancellationToken),
            (@lock, timeout, cancellationToken)
        );
    }

    #endregion

    #region ---- ISemaphoreProvider implementations ----

    public static ValueTask<THandle> AcquireAsync<THandle>(ISemaphoreProvider<THandle> @lock, TimeSpan? timeout, CancellationToken cancellationToken)
        where THandle : class, ISynchronizationHandle =>
        @lock.TryAcquireAsync(timeout, cancellationToken).ThrowTimeoutIfNull(@object: "semaphore");

    public static THandle Acquire<THandle>(ISemaphoreProvider<THandle> @lock, TimeSpan? timeout, CancellationToken cancellationToken)
        where THandle : class, ISynchronizationHandle =>
        TaskHelper.Run(
            state => AcquireAsync(state.@lock, state.timeout, state.cancellationToken),
            (@lock, timeout, cancellationToken)
        );

    public static THandle TryAcquire<THandle>(ISemaphoreProvider<THandle> @lock, TimeSpan timeout, CancellationToken cancellationToken)
        where THandle : class, ISynchronizationHandle =>
        TaskHelper.Run(
            state => state.@lock.TryAcquireAsync(state.timeout, state.cancellationToken),
            (@lock, timeout, cancellationToken)
        );

    #endregion

    private static Exception LockTimeout(string @object = null) => new TimeoutException($"Timeout exceeded when trying to acquire the {@object ?? "lock"}");

    private static async ValueTask<T> ThrowTimeoutIfNull<T>(this ValueTask<T> task, string @object = null) where T : class =>
        await task.ConfigureAwait(false) ?? throw LockTimeout(@object);
}

// ReSharper disable once UnusedTypeParameter
internal static class TaskConversion<T>
{
    public static ValueTaskConversion ValueTask => default;

    public readonly struct ValueTaskConversion
    {
    }
}