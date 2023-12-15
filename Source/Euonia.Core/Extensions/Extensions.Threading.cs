using System.ComponentModel;
using Nerosoft.Euonia.Threading;

public static partial class Extensions
{
    /// <summary>
    /// Attempts to complete a <see cref="TaskCompletionSource{TResult}"/>, propagating the completion of <paramref name="task"/>.
    /// </summary>
    /// <typeparam name="TResult">The type of the result of the target asynchronous operation.</typeparam>
    /// <typeparam name="TSourceResult">The type of the result of the source asynchronous operation.</typeparam>
    /// <param name="this">The task completion source. May not be <c>null</c>.</param>
    /// <param name="task">The task. May not be <c>null</c>.</param>
    /// <returns><c>true</c> if this method completed the task completion source; <c>false</c> if it was already completed.</returns>
    public static bool TryCompleteFromCompletedTask<TResult, TSourceResult>(this TaskCompletionSource<TResult> @this, Task<TSourceResult> task) where TSourceResult : TResult
    {
#if NETSTANDARD
		if (@this == null)
		{
			throw new ArgumentNullException(nameof(@this));
		}

		if (task == null)
		{
			throw new ArgumentNullException(nameof(task));
		} 
#else
		ArgumentNullException.ThrowIfNull(@this);
		ArgumentNullException.ThrowIfNull(task);
#endif

		if (task.IsFaulted)
        {
            return @this.TrySetException(task.Exception.InnerExceptions);
        }

        if (task.IsCanceled)
        {
            try
            {
                task.WaitAndUnwrapException();
            }
            catch (OperationCanceledException exception)
            {
                var token = exception.CancellationToken;
                return token.IsCancellationRequested ? @this.TrySetCanceled(token) : @this.TrySetCanceled();
            }
        }

        return @this.TrySetResult(task.Result);
    }

    /// <summary>
    /// Attempts to complete a <see cref="TaskCompletionSource{TResult}"/>, propagating the completion of <paramref name="task"/> but using the result value from <paramref name="resultFunc"/> if the task completed successfully.
    /// </summary>
    /// <typeparam name="TResult">The type of the result of the target asynchronous operation.</typeparam>
    /// <param name="this">The task completion source. May not be <c>null</c>.</param>
    /// <param name="task">The task. May not be <c>null</c>.</param>
    /// <param name="resultFunc">A delegate that returns the result with which to complete the task completion source, if the task completed successfully. May not be <c>null</c>.</param>
    /// <returns><c>true</c> if this method completed the task completion source; <c>false</c> if it was already completed.</returns>
    public static bool TryCompleteFromCompletedTask<TResult>(this TaskCompletionSource<TResult> @this, Task task, Func<TResult> resultFunc)
    {
#if NETSTANDARD
		if (@this == null)
		{
			throw new ArgumentNullException(nameof(@this));
		}
		if (task == null)
		{
			throw new ArgumentNullException(nameof(task));
		}
		if (resultFunc == null)
		{
			throw new ArgumentNullException(nameof(resultFunc));
		} 
#else
		ArgumentNullException.ThrowIfNull(@this);
		ArgumentNullException.ThrowIfNull(task);
		ArgumentNullException.ThrowIfNull(resultFunc);
#endif

		if (task.IsFaulted)
		{
			return @this.TrySetException(task.Exception.InnerExceptions);
		}
        if (task.IsCanceled)
        {
            try
            {
                task.WaitAndUnwrapException();
            }
            catch (OperationCanceledException exception)
            {
                var token = exception.CancellationToken;
                return token.IsCancellationRequested ? @this.TrySetCanceled(token) : @this.TrySetCanceled();
            }
        }

        return @this.TrySetResult(resultFunc());
    }

    /// <summary>
    /// Creates a new TCS for use with async code, and which forces its continuations to execute asynchronously.
    /// </summary>
    /// <typeparam name="TResult">The type of the result of the TCS.</typeparam>
    internal static TaskCompletionSource<TResult> CreateAsyncTaskSource<TResult>()
    {
        return new TaskCompletionSource<TResult>(TaskCreationOptions.RunContinuationsAsynchronously);
    }

    /// <summary>
    /// Asynchronously waits for the task to complete, or for the cancellation token to be canceled.
    /// </summary>
    /// <param name="this">The task to wait for. May not be <c>null</c>.</param>
    /// <param name="cancellationToken">The cancellation token that cancels the wait.</param>
    public static Task WaitAsync(this Task @this, CancellationToken cancellationToken)
    {
#if NETSTANDARD
		if (@this == null)
		{
			throw new ArgumentNullException(nameof(@this));
		} 
#else
		ArgumentNullException.ThrowIfNull(@this);
#endif

		if (!cancellationToken.CanBeCanceled)
        {
            return @this;
        }

        if (cancellationToken.IsCancellationRequested)
        {
            return Task.FromCanceled(cancellationToken);
        }

        return DoWaitAsync(@this, cancellationToken);
    }

    private static async Task DoWaitAsync(Task task, CancellationToken cancellationToken)
    {
		using (var cancelTaskSource = new CancellationTokenTaskSource<object>(cancellationToken))
		{
			await await Task.WhenAny(task, cancelTaskSource.Task).ConfigureAwait(false);
		}
    }

    /// <summary>
    /// Asynchronously waits for the task to complete, or for the cancellation token to be canceled.
    /// </summary>
    /// <typeparam name="TResult">The type of the task result.</typeparam>
    /// <param name="this">The task to wait for. May not be <c>null</c>.</param>
    /// <param name="cancellationToken">The cancellation token that cancels the wait.</param>
    public static Task<TResult> WaitAsync<TResult>(this Task<TResult> @this, CancellationToken cancellationToken)
    {
#if NETSTANDARD
		if (@this == null)
		{
			throw new ArgumentNullException(nameof(@this));
		} 
#else
		ArgumentNullException.ThrowIfNull(@this);
#endif

		if (!cancellationToken.CanBeCanceled)
		{
			return @this;
		}

		if (cancellationToken.IsCancellationRequested)
		{
			return Task.FromCanceled<TResult>(cancellationToken);
		}

		return DoWaitAsync(@this, cancellationToken);
    }

    private static async Task<TResult> DoWaitAsync<TResult>(Task<TResult> task, CancellationToken cancellationToken)
    {
        using (var cancelTaskSource = new CancellationTokenTaskSource<TResult>(cancellationToken))
		{
			return await await Task.WhenAny(task, cancelTaskSource.Task).ConfigureAwait(false);
		}
	}

    /// <summary>
    /// Asynchronously waits for any of the source tasks to complete, or for the cancellation token to be canceled.
    /// </summary>
    /// <param name="this">The tasks to wait for. May not be <c>null</c>.</param>
    /// <param name="cancellationToken">The cancellation token that cancels the wait.</param>
    public static Task<Task> WhenAny(this IEnumerable<Task> @this, CancellationToken cancellationToken)
    {
#if NETSTANDARD
		if (@this == null)
		{
			throw new ArgumentNullException(nameof(@this));
		}
#else
		ArgumentNullException.ThrowIfNull(@this);
#endif

		return Task.WhenAny(@this).WaitAsync(cancellationToken);
    }

    /// <summary>
    /// Asynchronously waits for any of the source tasks to complete.
    /// </summary>
    /// <param name="this">The tasks to wait for. May not be <c>null</c>.</param>
    public static Task<Task> WhenAny(this IEnumerable<Task> @this)
    {
#if NETSTANDARD
		if (@this == null)
		{
			throw new ArgumentNullException(nameof(@this));
		} 
#else
		ArgumentNullException.ThrowIfNull(@this);
#endif

		return Task.WhenAny(@this);
    }

    /// <summary>
    /// Asynchronously waits for any of the source tasks to complete, or for the cancellation token to be canceled.
    /// </summary>
    /// <typeparam name="TResult">The type of the task results.</typeparam>
    /// <param name="this">The tasks to wait for. May not be <c>null</c>.</param>
    /// <param name="cancellationToken">The cancellation token that cancels the wait.</param>
    public static Task<Task<TResult>> WhenAny<TResult>(this IEnumerable<Task<TResult>> @this, CancellationToken cancellationToken)
    {
#if NETSTANDARD
		if (@this == null)
		{
			throw new ArgumentNullException(nameof(@this));
		} 
#else
		ArgumentNullException.ThrowIfNull(@this);
#endif

		return Task.WhenAny(@this).WaitAsync(cancellationToken);
    }

    /// <summary>
    /// Asynchronously waits for any of the source tasks to complete.
    /// </summary>
    /// <typeparam name="TResult">The type of the task results.</typeparam>
    /// <param name="this">The tasks to wait for. May not be <c>null</c>.</param>
    public static Task<Task<TResult>> WhenAny<TResult>(this IEnumerable<Task<TResult>> @this)
    {
#if NETSTANDARD
		if (@this == null)
		{
			throw new ArgumentNullException(nameof(@this));
		} 
#else
		ArgumentNullException.ThrowIfNull(@this);
#endif

		return Task.WhenAny(@this);
    }

    /// <summary>
    /// Asynchronously waits for all of the source tasks to complete.
    /// </summary>
    /// <param name="this">The tasks to wait for. May not be <c>null</c>.</param>
    public static Task WhenAll(this IEnumerable<Task> @this)
    {
#if NETSTANDARD
		if (@this == null)
		{
			throw new ArgumentNullException(nameof(@this));
		} 
#else
		ArgumentNullException.ThrowIfNull(@this);
#endif

		return Task.WhenAll(@this);
    }

    /// <summary>
    /// Asynchronously waits for all of the source tasks to complete.
    /// </summary>
    /// <typeparam name="TResult">The type of the task results.</typeparam>
    /// <param name="this">The tasks to wait for. May not be <c>null</c>.</param>
    public static Task<TResult[]> WhenAll<TResult>(this IEnumerable<Task<TResult>> @this)
    {
#if NETSTANDARD
		if (@this == null)
		{
			throw new ArgumentNullException(nameof(@this));
		} 
#else
		ArgumentNullException.ThrowIfNull(@this);
#endif

		return Task.WhenAll(@this);
    }

    /// <summary>
    /// DANGEROUS! Ignores the completion of this task. Also ignores exceptions.
    /// </summary>
    /// <param name="this">The task to ignore.</param>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static async void Ignore(this Task @this)
    {
        try
        {
            await @this.ConfigureAwait(false);
        }
        catch
        {
            // ignored
        }
    }

    /// <summary>
    /// DANGEROUS! Ignores the completion and results of this task. Also ignores exceptions.
    /// </summary>
    /// <param name="this">The task to ignore.</param>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static async void Ignore<T>(this Task<T> @this)
    {
        try
        {
            await @this.ConfigureAwait(false);
        }
        catch
        {
            // ignored
        }
    }

    /// <summary>
    /// Creates a new collection of tasks that complete in order.
    /// </summary>
    /// <typeparam name="T">The type of the results of the tasks.</typeparam>
    /// <param name="this">The tasks to order by completion. May not be <c>null</c>.</param>
    public static List<Task<T>> OrderByCompletion<T>(this IEnumerable<Task<T>> @this)
    {
#if NETSTANDARD
		if (@this == null)
		{
			throw new ArgumentNullException(nameof(@this));
		} 
#else
		ArgumentNullException.ThrowIfNull(@this);
#endif
		// This is a combination of Jon Skeet's approach and Stephen Toub's approach:
		//  http://msmvps.com/blogs/jon_skeet/archive/2012/01/16/eduasync-part-19-ordering-by-completion-ahead-of-time.aspx
		//  http://blogs.msdn.com/b/pfxteam/archive/2012/08/02/processing-tasks-as-they-complete.aspx

		// Reify the source task sequence. TODO: better reification.
		var taskArray = @this.ToArray();

        // Allocate a TCS array and an array of the resulting tasks.
        var numTasks = taskArray.Length;
        var tcs = new TaskCompletionSource<T>[numTasks];
        var ret = new List<Task<T>>(numTasks);

        // As each task completes, complete the next tcs.
        var lastIndex = -1;
		// ReSharper disable once ConvertToLocalFunction
		void Continuation(Task<T> task)
		{
			var index = Interlocked.Increment(ref lastIndex);
			tcs[index].TryCompleteFromCompletedTask(task);
		}

		// Fill out the arrays and attach the continuations.
		for (var i = 0; i != numTasks; ++i)
        {
            tcs[i] = new TaskCompletionSource<T>();
            ret.Add(tcs[i].Task);
            taskArray[i].ContinueWith(Continuation, CancellationToken.None, TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.DenyChildAttach, TaskScheduler.Default);
        }

        return ret;
    }

    /// <summary>
    /// Creates a new collection of tasks that complete in order.
    /// </summary>
    /// <param name="this">The tasks to order by completion. May not be <c>null</c>.</param>
    public static List<Task> OrderByCompletion(this IEnumerable<Task> @this)
    {
#if NETSTANDARD
		if (@this == null)
		{
			throw new ArgumentNullException(nameof(@this));
		} 
#else
		ArgumentNullException.ThrowIfNull(@this);
#endif
		// This is a combination of Jon Skeet's approach and Stephen Toub's approach:
		//  http://msmvps.com/blogs/jon_skeet/archive/2012/01/16/eduasync-part-19-ordering-by-completion-ahead-of-time.aspx
		//  http://blogs.msdn.com/b/pfxteam/archive/2012/08/02/processing-tasks-as-they-complete.aspx

		// Reify the source task sequence. TODO: better reification.
		var taskArray = @this.ToArray();

        // Allocate a TCS array and an array of the resulting tasks.
        var numTasks = taskArray.Length;
        var tcs = new TaskCompletionSource<object>[numTasks];
        var ret = new List<Task>(numTasks);

        // As each task completes, complete the next tcs.
        var lastIndex = -1;
		// ReSharper disable once ConvertToLocalFunction
		void Continuation(Task task)
		{
			var index = Interlocked.Increment(ref lastIndex);
			tcs[index].TryCompleteFromCompletedTask(task, NullResultFunc);
		}

		// Fill out the arrays and attach the continuations.
		for (var i = 0; i != numTasks; ++i)
        {
            tcs[i] = new TaskCompletionSource<object>();
            ret.Add(tcs[i].Task);
            taskArray[i].ContinueWith(Continuation, CancellationToken.None, TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.DenyChildAttach, TaskScheduler.Default);
        }

        return ret;
    }

    /// <summary>
    /// Waits for the task to complete, unwrapping any exceptions.
    /// </summary>
    /// <param name="task">The task. May not be <c>null</c>.</param>
    public static void WaitAndUnwrapException(this Task task)
    {
#if NETSTANDARD
		if (task == null)
		{
			throw new ArgumentNullException(nameof(task));
		} 
#else
		ArgumentNullException.ThrowIfNull(task);
#endif

		task.GetAwaiter().GetResult();
    }

    /// <summary>
    /// Waits for the task to complete, unwrapping any exceptions.
    /// </summary>
    /// <param name="task">The task. May not be <c>null</c>.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <exception cref="OperationCanceledException">The <paramref name="cancellationToken"/> was cancelled before the <paramref name="task"/> completed, or the <paramref name="task"/> raised an <see cref="OperationCanceledException"/>.</exception>
    public static void WaitAndUnwrapException(this Task task, CancellationToken cancellationToken)
    {
#if NETSTANDARD
		if (task == null)
		{
			throw new ArgumentNullException(nameof(task));
		} 
#else
		ArgumentNullException.ThrowIfNull(task);
#endif

		try
		{
            task.Wait(cancellationToken);
        }
        catch (AggregateException ex)
        {
            throw ex.InnerException.PrepareForRethrow();
        }
    }

    /// <summary>
    /// Waits for the task to complete, unwrapping any exceptions.
    /// </summary>
    /// <typeparam name="TResult">The type of the result of the task.</typeparam>
    /// <param name="task">The task. May not be <c>null</c>.</param>
    /// <returns>The result of the task.</returns>
    public static TResult WaitAndUnwrapException<TResult>(this Task<TResult> task)
    {
#if NETSTANDARD
		if (task == null)
		{
			throw new ArgumentNullException(nameof(task));
		} 
#else
		ArgumentNullException.ThrowIfNull(task);
#endif

		return task.GetAwaiter().GetResult();
    }

    /// <summary>
    /// Waits for the task to complete, unwrapping any exceptions.
    /// </summary>
    /// <typeparam name="TResult">The type of the result of the task.</typeparam>
    /// <param name="task">The task. May not be <c>null</c>.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The result of the task.</returns>
    /// <exception cref="OperationCanceledException">The <paramref name="cancellationToken"/> was cancelled before the <paramref name="task"/> completed, or the <paramref name="task"/> raised an <see cref="OperationCanceledException"/>.</exception>
    public static TResult WaitAndUnwrapException<TResult>(this Task<TResult> task, CancellationToken cancellationToken)
    {
#if NETSTANDARD
		if (task == null)
		{
			throw new ArgumentNullException(nameof(task));
		} 
#else
		ArgumentNullException.ThrowIfNull(task);
#endif

		try
		{
            task.Wait(cancellationToken);
            return task.Result;
        }
        catch (AggregateException ex)
        {
            throw ex.InnerException.PrepareForRethrow();
        }
    }

    /// <summary>
    /// Waits for the task to complete, but does not raise task exceptions. The task exception (if any) is unobserved.
    /// </summary>
    /// <param name="task">The task. May not be <c>null</c>.</param>
    public static void WaitWithoutException(this Task task)
    {
#if NETSTANDARD
		if (task == null)
		{
			throw new ArgumentNullException(nameof(task));
		}
#else
		ArgumentNullException.ThrowIfNull(task);
#endif

		try
        {
            task.Wait();
        }
        catch (AggregateException)
        {
        }
    }

    /// <summary>
    /// Waits for the task to complete, but does not raise task exceptions. The task exception (if any) is unobserved.
    /// </summary>
    /// <param name="task">The task. May not be <c>null</c>.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <exception cref="OperationCanceledException">The <paramref name="cancellationToken"/> was cancelled before the <paramref name="task"/> completed.</exception>
    public static void WaitWithoutException(this Task task, CancellationToken cancellationToken)
    {
#if NETSTANDARD
		if (task == null)
		{
			throw new ArgumentNullException(nameof(task));
		} 
#else
		ArgumentNullException.ThrowIfNull(task);
#endif

		try
		{
            task.Wait(cancellationToken);
        }
        catch (AggregateException)
        {
            cancellationToken.ThrowIfCancellationRequested();
        }
    }

    private static Func<object> NullResultFunc { get; } = () => null;

    #region SynchronizationContext

    /// <summary>
    /// Synchronously executes a delegate on this synchronization context.
    /// </summary>
    /// <param name="context">The synchronization context.</param>
    /// <param name="action">The delegate to execute.</param>
    public static void Send(this SynchronizationContext context, Action action)
    {
        context.Send(state => ((Action)state!)(), action);
    }

    /// <summary>
    /// Synchronously executes a delegate on this synchronization context and returns its result.
    /// </summary>
    /// <typeparam name="T">The type of the result.</typeparam>
    /// <param name="context">The synchronization context.</param>
    /// <param name="action">The delegate to execute.</param>
    public static T Send<T>(this SynchronizationContext context, Func<T> action)
    {
        var result = default(T);
        context.Send(state =>
        {
            result = ((Func<T>)state!)();
        }, action);
        return result;
    }

    /// <summary>
    /// Asynchronously executes a delegate on this synchronization context.
    /// </summary>
    /// <param name="context">The synchronization context.</param>
    /// <param name="action">The delegate to execute.</param>
    public static Task PostAsync(this SynchronizationContext context, Action action)
    {
        var taskCompletionSource = CreateAsyncTaskSource<object>();
        context.Post(state =>
        {
            try
            {
                ((Action)state!)();
                taskCompletionSource.TrySetResult(null);
            }
            catch (OperationCanceledException ex)
            {
                taskCompletionSource.TrySetCanceled(ex.CancellationToken);
            }
            catch (Exception ex)
            {
                taskCompletionSource.TrySetException(ex);
            }
        }, action);
        return taskCompletionSource.Task;
    }

    /// <summary>
    /// Asynchronously executes a delegate on this synchronization context and returns its result.
    /// </summary>
    /// <typeparam name="T">The type of the result.</typeparam>
    /// <param name="context">The synchronization context.</param>
    /// <param name="action">The delegate to execute.</param>
    public static Task<T> PostAsync<T>(this SynchronizationContext context, Func<T> action)
    {
        var taskCompletionSource = CreateAsyncTaskSource<T>();
        context.Post(state =>
        {
            try
            {
                taskCompletionSource.SetResult(((Func<T>)state!)());
            }
            catch (OperationCanceledException ex)
            {
                taskCompletionSource.TrySetCanceled(ex.CancellationToken);
            }
            catch (Exception ex)
            {
                taskCompletionSource.TrySetException(ex);
            }
        }, action);
        return taskCompletionSource.Task;
    }

    /// <summary>
    /// Asynchronously executes an asynchronous delegate on this synchronization context.
    /// </summary>
    /// <param name="context">The synchronization context.</param>
    /// <param name="action">The delegate to execute.</param>
    public static Task PostAsync(this SynchronizationContext context, Func<Task> action)
    {
        var taskCompletionSource = CreateAsyncTaskSource<object>();

        async void PostCallback(object state)
        {
            try
            {
                await ((Func<Task>)state!)().ConfigureAwait(false);
                taskCompletionSource.TrySetResult(null);
            }
            catch (OperationCanceledException ex)
            {
                taskCompletionSource.TrySetCanceled(ex.CancellationToken);
            }
            catch (Exception ex)
            {
                taskCompletionSource.TrySetException(ex);
            }
        }

        context.Post(PostCallback, action);
        return taskCompletionSource.Task;
    }

    /// <summary>
    /// Asynchronously executes an asynchronous delegate on this synchronization context and returns its result.
    /// </summary>
    /// <typeparam name="T">The type of the result.</typeparam>
    /// <param name="context">The synchronization context.</param>
    /// <param name="action">The delegate to execute.</param>
    public static Task<T> PostAsync<T>(this SynchronizationContext context, Func<Task<T>> action)
    {
        var taskCompletionSource = CreateAsyncTaskSource<T>();

        async void PostCallback(object state)
        {
            try
            {
                taskCompletionSource.SetResult(await ((Func<Task<T>>)state!)().ConfigureAwait(false));
            }
            catch (OperationCanceledException ex)
            {
                taskCompletionSource.TrySetCanceled(ex.CancellationToken);
            }
            catch (Exception ex)
            {
                taskCompletionSource.TrySetException(ex);
            }
        }

        context.Post(PostCallback, action);
        return taskCompletionSource.Task;
    }

    #endregion

    #region TaskFactory

    /// <summary>
    /// Queues work to the task factory and returns a <see cref="Task"/> representing that work. If the task factory does not specify a task scheduler, the thread pool task scheduler is used.
    /// </summary>
    /// <param name="this">The <see cref="TaskFactory"/>. May not be <c>null</c>.</param>
    /// <param name="action">The action delegate to execute. May not be <c>null</c>.</param>
    /// <returns>The started task.</returns>
    public static Task Run(this TaskFactory @this, Action action)
    {
#if NETSTANDARD
		if (@this == null)
		{
			throw new ArgumentNullException(nameof(@this));
		}
		if (action == null)
		{
			throw new ArgumentNullException(nameof(action));
		} 
#else
		ArgumentNullException.ThrowIfNull(@this);
		ArgumentNullException.ThrowIfNull(action);
#endif

		return @this.StartNew(action, @this.CancellationToken, @this.CreationOptions | TaskCreationOptions.DenyChildAttach, @this.Scheduler ?? TaskScheduler.Default);
    }

    /// <summary>
    /// Queues work to the task factory and returns a <see cref="Task{TResult}"/> representing that work. If the task factory does not specify a task scheduler, the thread pool task scheduler is used.
    /// </summary>
    /// <param name="this">The <see cref="TaskFactory"/>. May not be <c>null</c>.</param>
    /// <param name="action">The action delegate to execute. May not be <c>null</c>.</param>
    /// <returns>The started task.</returns>
    public static Task<TResult> Run<TResult>(this TaskFactory @this, Func<TResult> action)
    {
#if NETSTANDARD
		if (@this == null)
		{
			throw new ArgumentNullException(nameof(@this));
		}

		if (action == null)
		{
			throw new ArgumentNullException(nameof(action));
		} 
#else
		ArgumentNullException.ThrowIfNull(@this);
		ArgumentNullException.ThrowIfNull(action);
#endif

		return @this.StartNew(action, @this.CancellationToken, @this.CreationOptions | TaskCreationOptions.DenyChildAttach, @this.Scheduler ?? TaskScheduler.Default);
    }

    /// <summary>
    /// Queues work to the task factory and returns a proxy <see cref="Task"/> representing that work. If the task factory does not specify a task scheduler, the thread pool task scheduler is used.
    /// </summary>
    /// <param name="this">The <see cref="TaskFactory"/>. May not be <c>null</c>.</param>
    /// <param name="action">The action delegate to execute. May not be <c>null</c>.</param>
    /// <returns>The started task.</returns>
    public static Task Run(this TaskFactory @this, Func<Task> action)
    {
#if NETSTANDARD
		if (@this == null)
		{
			throw new ArgumentNullException(nameof(@this));
		}

		if (action == null)
		{
			throw new ArgumentNullException(nameof(action));
		} 
#else
		ArgumentNullException.ThrowIfNull(@this);
		ArgumentNullException.ThrowIfNull(action);
#endif

		return @this.StartNew(action, @this.CancellationToken, @this.CreationOptions | TaskCreationOptions.DenyChildAttach, @this.Scheduler ?? TaskScheduler.Default).Unwrap();
    }

    /// <summary>
    /// Queues work to the task factory and returns a proxy <see cref="Task{TResult}"/> representing that work. If the task factory does not specify a task scheduler, the thread pool task scheduler is used.
    /// </summary>
    /// <param name="this">The <see cref="TaskFactory"/>. May not be <c>null</c>.</param>
    /// <param name="action">The action delegate to execute. May not be <c>null</c>.</param>
    /// <returns>The started task.</returns>
    public static Task<TResult> Run<TResult>(this TaskFactory @this, Func<Task<TResult>> action)
    {
#if NETSTANDARD
		if (@this == null)
		{
			throw new ArgumentNullException(nameof(@this));
		}
		if (action == null)
		{
			throw new ArgumentNullException(nameof(action));
		} 
#else
		ArgumentNullException.ThrowIfNull(@this);
		ArgumentNullException.ThrowIfNull(action);
#endif
		return @this.StartNew(action, @this.CancellationToken, @this.CreationOptions | TaskCreationOptions.DenyChildAttach, @this.Scheduler ?? TaskScheduler.Default).Unwrap();
    }

    #endregion
    
    /// <summary>
    /// Converts a <see cref="Task"/> to a <see cref="ValueTask"/>.
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="task"></param>
    /// <returns></returns>
    public static async ValueTask ConvertToVoid<TResult>(this ValueTask<TResult> task) => await task.ConfigureAwait(false);

    /// <summary>
    /// Converts a <see cref="Task{T}"/> to a <see cref="ValueTask{T}"/>.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="task"></param>
    /// <returns></returns>
    public static ValueTask<T> AsValueTask<T>(this Task<T> task) => new(task);

    /// <summary>
    /// Converts a <see cref="Task"/> to a <see cref="ValueTask"/>.
    /// </summary>
    /// <param name="task"></param>
    /// <returns></returns>
    public static ValueTask AsValueTask(this Task task) => new(task);

    /// <summary>
    /// Converts a value to a <see cref="ValueTask{T}"/>.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="value"></param>
    /// <returns></returns>
    public static ValueTask<T> AsValueTask<T>(this T value) => new(value);
}