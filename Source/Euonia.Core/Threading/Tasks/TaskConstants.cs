namespace Nerosoft.Euonia.Threading;

/// <summary>
/// Provides completed task constants.
/// </summary>
public static class TaskConstants
{
    /// <summary>
    /// A task that has been completed with the value <c>true</c>.
    /// </summary>
    public static Task<bool> BooleanTrue => Task.FromResult(true);

    /// <summary>
    /// A task that has been completed with the value <c>false</c>.
    /// </summary>
    public static Task<bool> BooleanFalse => TaskConstants<bool>.Default;

    /// <summary>
    /// A task that has been completed with the value <c>0</c>.
    /// </summary>
    public static Task<int> Int32Zero => TaskConstants<int>.Default;

    /// <summary>
    /// A task that has been completed with the value <c>-1</c>.
    /// </summary>
    public static Task<int> Int32NegativeOne => Task.FromResult(-1);

    /// <summary>
    /// A <see cref="Task"/> that has been completed.
    /// </summary>
    public static Task Completed => Task.CompletedTask;

    /// <summary>
    /// A task that has been canceled.
    /// </summary>
    public static Task Canceled => TaskConstants<object>.Canceled;
}

/// <summary>
/// Provides completed task constants.
/// </summary>
/// <typeparam name="T">The type of the task result.</typeparam>
public static class TaskConstants<T>
{
    /// <summary>
    /// A task that has been completed with the default value of <typeparamref name="T"/>.
    /// </summary>
    public static Task<T> Default => Task.FromResult(default(T));

    /// <summary>
    /// A task that has been canceled.
    /// </summary>
    public static Task<T> Canceled => Task.FromCanceled<T>(new CancellationToken(true));
}