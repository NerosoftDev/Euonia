namespace Nerosoft.Euonia.Disposing;

/// <summary>
/// 
/// </summary>
public sealed class AsyncNoopDisposable : IAsyncDisposable
{
    /// <summary>
    /// Does nothing.
    /// </summary>
    public ValueTask DisposeAsync() => new ValueTask();
}