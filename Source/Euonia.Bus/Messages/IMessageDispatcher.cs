namespace Nerosoft.Euonia.Bus;

/// <summary>
/// Interface IMessageDispatcher
/// Implements the <see cref="IDisposable" />
/// </summary>
/// <seealso cref="IDisposable" />
public interface IMessageDispatcher : IDisposable
{
    /// <summary>
    /// Occurs when [message subscribed].
    /// </summary>
    event EventHandler<MessageSubscribedEventArgs> MessageSubscribed;

    /// <summary>
    /// Occurs when [message dispatched].
    /// </summary>
    event EventHandler<MessageDispatchedEventArgs> MessageDispatched;
}