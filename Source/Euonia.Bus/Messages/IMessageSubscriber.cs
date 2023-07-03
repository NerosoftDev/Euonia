namespace Nerosoft.Euonia.Bus;

/// <summary>
/// Interface IMessageSubscriber
/// Implements the <see cref="IDisposable" />
/// </summary>
/// <seealso cref="IDisposable" />
public interface IMessageSubscriber : IDisposable
{
    /// <summary>
    /// Occurs when [message received].
    /// </summary>
    event EventHandler<MessageReceivedEventArgs> MessageReceived;

    /// <summary>
    /// Occurs when [message acknowledged].
    /// </summary>
    event EventHandler<MessageAcknowledgedEventArgs> MessageAcknowledged;

    /// <summary>
    /// Subscribes the specified message type.
    /// </summary>
    /// <param name="eventType"></param>
    /// <param name="handlerType"></param>
    void Subscribe(Type eventType, Type handlerType);
}