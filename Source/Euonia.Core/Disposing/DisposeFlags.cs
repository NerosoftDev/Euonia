namespace Nerosoft.Euonia.Disposing;

/// <summary>
/// 
/// </summary>
[Flags]
public enum DisposeFlags
{
    /// <summary>
    /// Execute multiple asynchronous disposal methods concurrently. All asynchronous disposal methods are started, and then asynchronously wait for all of them to complete.
    /// </summary>
    ExecuteConcurrently = 1,

    /// <summary>
    /// Execute multiple asynchronous disposal methods serially. Each asynchronous disposal method will not start until the previous one has completed.
    /// </summary>
    ExecuteSerially = 2,
}