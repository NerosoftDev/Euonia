using System.ComponentModel;

namespace Nerosoft.Euonia.Threading;

/// <summary>
/// Helper class to wrap around a Task to provide more information usable for UI data binding scenarios. As discussed in MSDN Magazine: https://msdn.microsoft.com/magazine/dn605875.
/// </summary>
/// <typeparam name="TResult">Type of result returned by task.</typeparam>
public sealed class NotifyTaskCompletion<TResult> : INotifyPropertyChanged
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NotifyTaskCompletion{TResult}"/> class.
    /// </summary>
    /// <param name="task">Task to wait on.</param>
    public NotifyTaskCompletion(Task<TResult> task)
    {
        Task = task;
        if (!task.IsCompleted)
        {
            TaskCompletion = WatchTaskAsync(task);
        }
    }

    private async Task WatchTaskAsync(Task task)
    {
        try
        {
            await task;
        }
        catch
        {
            //
        }

        if (PropertyChanged == null)
        {
            return;
        }

        PropertyChanged.Invoke(this, new PropertyChangedEventArgs(nameof(Status)));
        PropertyChanged.Invoke(this, new PropertyChangedEventArgs(nameof(IsCompleted)));
        PropertyChanged.Invoke(this, new PropertyChangedEventArgs(nameof(IsNotCompleted)));

        if (task.IsCanceled)
        {
            PropertyChanged.Invoke(this, new PropertyChangedEventArgs(nameof(IsCanceled)));
        }
        else if (task.IsFaulted)
        {
            PropertyChanged.Invoke(this, new PropertyChangedEventArgs(nameof(IsFaulted)));
            PropertyChanged.Invoke(this, new PropertyChangedEventArgs(nameof(Exception)));
            PropertyChanged.Invoke(this, new PropertyChangedEventArgs(nameof(InnerException)));
            PropertyChanged.Invoke(this, new PropertyChangedEventArgs(nameof(ErrorMessage)));
        }
        else
        {
            PropertyChanged.Invoke(this, new PropertyChangedEventArgs(nameof(IsSuccessfullyCompleted)));
            PropertyChanged.Invoke(this, new PropertyChangedEventArgs(nameof(Result)));
        }
    }

    /// <summary>
    /// Gets the task that is being waited on.
    /// </summary>
    public Task<TResult> Task { get; }

    /// <summary>
    /// Gets the task wrapper task.
    /// </summary>
    public Task TaskCompletion { get; }

    /// <summary>
    /// Gets the result of the given task.
    /// </summary>
    public TResult Result => Task.Status == TaskStatus.RanToCompletion ? Task.Result : default;

    /// <summary>
    /// Gets the status of the task.
    /// </summary>
    public TaskStatus Status => Task.Status;

    /// <summary>
    /// Gets a value indicating whether the task is completed.
    /// </summary>
    public bool IsCompleted => Task.IsCompleted;

    /// <summary>
    /// Gets a value indicating whether the task is not completed.
    /// </summary>
    public bool IsNotCompleted => !Task.IsCompleted;

    /// <summary>
    /// Gets a value indicating whether the task was successfully completed.
    /// </summary>
    public bool IsSuccessfullyCompleted => Task.Status == TaskStatus.RanToCompletion;

    /// <summary>
    /// Gets a value indicating whether the task was cancelled.
    /// </summary>
    public bool IsCanceled => Task.IsCanceled;

    /// <summary>
    /// Gets a value indicating whether there was an error with the task.
    /// </summary>
    public bool IsFaulted => Task.IsFaulted;

    /// <summary>
    /// Gets the exception which occured on the task (if one occurred).
    /// </summary>
    public AggregateException Exception => Task.Exception;

    /// <summary>
    /// Gets the inner exception of the task.
    /// </summary>
    public Exception InnerException => Exception?.InnerException;

    /// <summary>
    /// Gets the error message of the task.
    /// </summary>
    public string ErrorMessage => InnerException?.Message ?? Exception.Message;

    /// <summary>
    /// PropertyChanged event.
    /// </summary>
    public event PropertyChangedEventHandler PropertyChanged;
}

/// <summary>
/// Helper class to wrap around a Task to provide more information usable for UI data binding scenarios. As discussed in MSDN Magazine: https://msdn.microsoft.com/magazine/dn605875.
/// </summary>
public sealed class NotifyTaskCompletion : INotifyPropertyChanged
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NotifyTaskCompletion"/> class.
    /// </summary>
    /// <param name="task">Task to wait on.</param>
    public NotifyTaskCompletion(Task task)
    {
        Task = task;
        if (!task.IsCompleted)
        {
            TaskCompletion = WatchTaskAsync(task);
        }
    }

    private async Task WatchTaskAsync(Task task)
    {
        try
        {
            await task;
        }
        catch
        {
            //
        }

        if (PropertyChanged == null)
        {
            return;
        }

        PropertyChanged.Invoke(this, new PropertyChangedEventArgs(nameof(Status)));
        PropertyChanged.Invoke(this, new PropertyChangedEventArgs(nameof(IsCompleted)));
        PropertyChanged.Invoke(this, new PropertyChangedEventArgs(nameof(IsNotCompleted)));

        if (task.IsCanceled)
        {
            PropertyChanged.Invoke(this, new PropertyChangedEventArgs(nameof(IsCanceled)));
        }
        else if (task.IsFaulted)
        {
            PropertyChanged.Invoke(this, new PropertyChangedEventArgs(nameof(IsFaulted)));
            PropertyChanged.Invoke(this, new PropertyChangedEventArgs(nameof(Exception)));
            PropertyChanged.Invoke(this, new PropertyChangedEventArgs(nameof(InnerException)));
            PropertyChanged.Invoke(this, new PropertyChangedEventArgs(nameof(ErrorMessage)));
        }
        else
        {
            PropertyChanged.Invoke(this, new PropertyChangedEventArgs(nameof(IsSuccessfullyCompleted)));
        }
    }

    /// <summary>
    /// Gets the task that is being waited on.
    /// </summary>
    public Task Task { get; }

    /// <summary>
    /// Gets the task wrapper task.
    /// </summary>
    public Task TaskCompletion { get; }

    /// <summary>
    /// Gets the status of the task.
    /// </summary>
    public TaskStatus Status => Task.Status;

    /// <summary>
    /// Gets a value indicating whether the task is completed.
    /// </summary>
    public bool IsCompleted => Task.IsCompleted;

    /// <summary>
    /// Gets a value indicating whether the task is not completed.
    /// </summary>
    public bool IsNotCompleted => !Task.IsCompleted;

    /// <summary>
    /// Gets a value indicating whether the task was successfully completed.
    /// </summary>
    public bool IsSuccessfullyCompleted => Task.Status == TaskStatus.RanToCompletion;

    /// <summary>
    /// Gets a value indicating whether the task was cancelled.
    /// </summary>
    public bool IsCanceled => Task.IsCanceled;

    /// <summary>
    /// Gets a value indicating whether there was an error with the task.
    /// </summary>
    public bool IsFaulted => Task.IsFaulted;

    /// <summary>
    /// Gets the exception which occured on the task (if one occurred).
    /// </summary>
    public AggregateException Exception => Task.Exception;

    /// <summary>
    /// Gets the inner exception of the task.
    /// </summary>
    public Exception InnerException => Exception?.InnerException;

    /// <summary>
    /// Gets the error message of the task.
    /// </summary>
    public string ErrorMessage => InnerException?.Message ?? Exception.Message;

    /// <summary>
    /// PropertyChanged event.
    /// </summary>
    public event PropertyChangedEventHandler PropertyChanged;
}