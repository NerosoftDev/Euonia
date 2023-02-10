namespace System;

/// <summary>
/// Represents the base class of the objects that can be disposed.
/// </summary>
/// <remarks>
/// This class provides the basic implementation of the disposable pattern in .NET.
/// </remarks>
public abstract class DisposableObject : IDisposable
{
    private readonly WeakEventManager _events = new();

    /// <summary>
    /// Occurs when current object has been disposed.
    /// </summary>
    public event EventHandler<DisposedEventArgs> Disposed
    {
        add => _events.AddEventHandler(value);
        remove => _events.RemoveEventHandler(value);
    }

    /// <summary>
    /// Finalizes an instance of the <see cref="DisposableObject"/> class.
    /// </summary>
    ~DisposableObject()
    {
        Dispose(false);
        InvokeDisposedEvent(this, new DisposedEventArgs());
    }

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
        InvokeDisposedEvent(this, new DisposedEventArgs());
    }

    /// <summary>
    /// Releases unmanaged and - optionally - managed resources.
    /// </summary>
    /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
    protected abstract void Dispose(bool disposing);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    protected virtual void InvokeDisposedEvent(object sender, DisposedEventArgs args)
    {
        _events.HandleEvent(sender, args, nameof(Disposed));
    }
}