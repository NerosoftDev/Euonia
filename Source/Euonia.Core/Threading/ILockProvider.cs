namespace Nerosoft.Euonia.Threading;

/// <summary>
/// Specifies contract of lock provider.
/// </summary>
public interface ILockProvider
{
    #region Acquire & AcquireAsync

    /// <summary>
    /// Acquires an exclusive lock.
    /// </summary>
    /// <param name="resource">The token.</param>
    /// <param name="timeout">The timeout.</param>
    /// <returns><c>true</c> if lock acquires success, otherwise <c>false</c>.</returns>
    bool Acquire(string resource, TimeSpan timeout);

    /// <summary>
    /// Acquires an exclusive lock for specified operation.
    /// </summary>
    /// <param name="resource">The token.</param>
    /// <param name="timeout">The timeout.</param>
    /// <param name="action">The action.</param>
    void Acquire(string resource, TimeSpan timeout, Action action);

    /// <summary>
    /// Acquires an exclusive lock for specified operation.
    /// </summary>
    /// <typeparam name="TArgument">The type of the t argument.</typeparam>
    /// <param name="resource">The token.</param>
    /// <param name="timeout">The timeout.</param>
    /// <param name="action">The action.</param>
    /// <param name="argument">The argument.</param>
    void Acquire<TArgument>(string resource, TimeSpan timeout, Action<TArgument> action, TArgument argument);

    /// <summary>
    /// Acquires an exclusive lock for specified operation.
    /// </summary>
    /// <typeparam name="TResult">The type of the t result.</typeparam>
    /// <param name="resource">The token.</param>
    /// <param name="timeout">The timeout.</param>
    /// <param name="action">The action.</param>
    /// <returns>TResult.</returns>
    TResult Acquire<TResult>(string resource, TimeSpan timeout, Func<TResult> action);

    /// <summary>
    /// Acquires an exclusive lock for specified operation.
    /// </summary>
    /// <typeparam name="TArgument">The type of the t argument.</typeparam>
    /// <typeparam name="TResult">The type of the t result.</typeparam>
    /// <param name="resource">The token.</param>
    /// <param name="timeout">The timeout.</param>
    /// <param name="action">The action.</param>
    /// <param name="argument">The argument.</param>
    /// <returns>TResult.</returns>
    TResult Acquire<TArgument, TResult>(string resource, TimeSpan timeout, Func<TArgument, TResult> action, TArgument argument);

    /// <summary>
    /// Acquires an exclusive lock asynchronously.
    /// </summary>
    /// <param name="resource">The token.</param>
    /// <param name="timeout">The timeout.</param>
    /// <returns>Task&lt;System.Boolean&gt;.</returns>
    Task<bool> AcquireAsync(string resource, TimeSpan timeout);

    /// <summary>
    /// Acquires an exclusive lock asynchronously for specified operation.
    /// </summary>
    /// <param name="resource">The token.</param>
    /// <param name="timeout">The timeout.</param>
    /// <param name="action">The action.</param>
    /// <returns>Task.</returns>
    Task AcquireAsync(string resource, TimeSpan timeout, Func<Task> action);

    /// <summary>
    /// Acquires an exclusive lock asynchronously for specified operation.
    /// </summary>
    /// <typeparam name="TArgument">The type of the t argument.</typeparam>
    /// <param name="resource">The token.</param>
    /// <param name="timeout">The timeout.</param>
    /// <param name="action">The action.</param>
    /// <param name="argument">The argument.</param>
    /// <returns>Task.</returns>
    Task AcquireAsync<TArgument>(string resource, TimeSpan timeout, Func<TArgument, Task> action, TArgument argument);

    /// <summary>
    /// Acquires an exclusive lock asynchronously for specified operation.
    /// </summary>
    /// <typeparam name="TResult">The type of the t result.</typeparam>
    /// <param name="resource">The token.</param>
    /// <param name="timeout">The timeout.</param>
    /// <param name="action">The action.</param>
    /// <returns>Task&lt;TResult&gt;.</returns>
    Task<TResult> AcquireAsync<TResult>(string resource, TimeSpan timeout, Func<Task<TResult>> action);

    /// <summary>
    /// Acquires an exclusive lock asynchronously for specified operation.
    /// </summary>
    /// <typeparam name="TArgument">The type of the t argument.</typeparam>
    /// <typeparam name="TResult">The type of the t result.</typeparam>
    /// <param name="resource">The token.</param>
    /// <param name="timeout">The timeout.</param>
    /// <param name="action">The action.</param>
    /// <param name="argument">The argument.</param>
    /// <returns>Task&lt;TResult&gt;.</returns>
    Task<TResult> AcquireAsync<TArgument, TResult>(string resource, TimeSpan timeout, Func<TArgument, Task<TResult>> action, TArgument argument);

    #endregion

    #region TryAcquire & TryAcquireAsync

    /// <summary>
    /// Attempts to acquire an exclusive lock.
    /// </summary>
    /// <param name="resource"></param>
    /// <param name="timeout"></param>
    /// <param name="action"></param>
    /// <param name="failureCallback"></param>
    void TryAcquire(string resource, TimeSpan timeout, Action action, Action failureCallback = null);

    /// <summary>
    /// Attempts to acquire an exclusive lock.
    /// </summary>
    /// <param name="resource"></param>
    /// <param name="timeout"></param>
    /// <param name="action"></param>
    /// <param name="argument"></param>
    /// <param name="failureCallback"></param>
    /// <typeparam name="TArgument"></typeparam>
    void TryAcquire<TArgument>(string resource, TimeSpan timeout, Action<TArgument> action, TArgument argument, Action failureCallback = null);

    /// <summary>
    /// Attempts to acquire an exclusive lock.
    /// </summary>
    /// <param name="resource"></param>
    /// <param name="timeout"></param>
    /// <param name="action"></param>
    /// <param name="failureCallback"></param>
    /// <typeparam name="TResult"></typeparam>
    /// <returns></returns>
    TResult TryAcquire<TResult>(string resource, TimeSpan timeout, Func<TResult> action, Action failureCallback = null);

    /// <summary>
    /// Attempts to acquire an exclusive lock.
    /// </summary>
    /// <param name="resource"></param>
    /// <param name="timeout"></param>
    /// <param name="action"></param>
    /// <param name="argument"></param>
    /// <param name="failureCallback"></param>
    /// <typeparam name="TArgument"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    /// <returns></returns>
    TResult TryAcquire<TArgument, TResult>(string resource, TimeSpan timeout, Func<TArgument, TResult> action, TArgument argument, Action failureCallback = null);

    /// <summary>
    /// Attempts to acquire an exclusive lock asynchronously.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="timeout"></param>
    /// <param name="action"></param>
    /// <param name="failureCallback"></param>
    /// <returns></returns>
    Task TryAcquireAsync(string source, TimeSpan timeout, Func<Task> action, Func<Task> failureCallback = null);

    /// <summary>
    /// Attempts to acquire an exclusive lock asynchronously.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="timeout"></param>
    /// <param name="action"></param>
    /// <param name="argument"></param>
    /// <param name="failureCallback"></param>
    /// <typeparam name="TArgument"></typeparam>
    /// <returns></returns>
    Task TryAcquireAsync<TArgument>(string source, TimeSpan timeout, Func<TArgument, Task> action, TArgument argument, Func<Task> failureCallback = null);

    /// <summary>
    /// Attempts to acquire an exclusive lock asynchronously.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="timeout"></param>
    /// <param name="action"></param>
    /// <param name="failureCallback"></param>
    /// <typeparam name="TResult"></typeparam>
    /// <returns></returns>
    Task<TResult> TryAcquireAsync<TResult>(string source, TimeSpan timeout, Func<Task<TResult>> action, Func<Task> failureCallback = null);

    /// <summary>
    /// Attempts to acquire an exclusive lock asynchronously.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="timeout"></param>
    /// <param name="action"></param>
    /// <param name="argument"></param>
    /// <param name="failureCallback"></param>
    /// <typeparam name="TArgument"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    /// <returns></returns>
    Task<TResult> TryAcquireAsync<TArgument, TResult>(string source, TimeSpan timeout, Func<TArgument, Task<TResult>> action, TArgument argument, Func<Task> failureCallback = null);

    #endregion

    #region Release

    /// <summary>
    /// Releases the specified token.
    /// </summary>
    /// <param name="resource">The token.</param>
    void Release(string resource);

    /// <summary>
    /// Releases asynchronously.
    /// </summary>
    /// <param name="resource">The token.</param>
    /// <returns>Task.</returns>
    Task ReleaseAsync(string resource);

    #endregion
}