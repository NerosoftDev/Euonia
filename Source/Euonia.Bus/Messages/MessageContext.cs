using Nerosoft.Euonia.Domain;

namespace Nerosoft.Euonia.Bus;

/// <summary>
/// The message context.
/// </summary>
public class MessageContext : IDisposable
{
    private readonly WeakEventManager _events = new();

    private bool disposedValue;

    public MessageContext()
    {
    }

    public MessageContext(IMessage message)
    {
        Message = message;
    }

    /// <summary>
    /// Invoked while message was handled and replied to dispatcher.
    /// </summary>
    public event EventHandler<MessageRepliedEventArgs> Replied
    {
        add => _events.AddEventHandler(value);
        remove => _events.RemoveEventHandler(value);
    }

    /// <summary>
    /// Invoke while message context disposed.
    /// </summary>
    public event EventHandler<MessageHandledEventArgs> Completed
    {
        add => _events.AddEventHandler(value);
        remove => _events.RemoveEventHandler(value);
    }

    public IMessage Message { get; set; }

    /// <summary>
    /// Replies message handling result to message dispatcher.
    /// </summary>
    /// <param name="message">The message to reply.</param>
    public void Reply(object message)
    {
        _events.HandleEvent(this, new MessageRepliedEventArgs(message), nameof(Replied));
    }

    /// <summary>
    /// Replies message handling result to message dispatcher.
    /// </summary>
    /// <typeparam name="TMessage">The type of the message.</typeparam>
    /// <param name="message">The message to reply.</param>
    public void Reply<TMessage>(TMessage message)
    {
        Reply((object)message);
    }

    /// <summary>
    /// Called after the message has been handled.
    /// This operate will raised up the <see cref="Completed"/> event.
    /// </summary>
    /// <param name="message"></param>
    public void Complete(IMessage message)
    {
        _events.HandleEvent(this, new MessageHandledEventArgs(message), nameof(Completed));
    }

    /// <summary>
    /// Called after the message has been handled.
    /// This operate will raised up the <see cref="Completed"/> event.
    /// </summary>
    /// <param name="message"></param>
    /// <param name="handlerType"></param>
    public void Complete(IMessage message, Type handlerType)
    {
        _events.HandleEvent(this, new MessageHandledEventArgs(message) { HandlerType = handlerType }, nameof(Completed));
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                Complete(Message);
            }

            _events.RemoveEventHandlers();
            disposedValue = true;
        }
    }

    ~MessageContext()
    {
        Dispose(disposing: false);
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}