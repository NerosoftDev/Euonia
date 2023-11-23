namespace Nerosoft.Euonia.Bus;

/// <summary>
/// Interface ISubscriber
/// Implements the <see cref="IDisposable" />
/// </summary>
/// <seealso cref="IDisposable" />
public interface ISubscriber : IDisposable
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
	/// Occurs when [message subscribed].
	/// </summary>
	event EventHandler<MessageSubscribedEventArgs> MessageSubscribed;

	/// <summary>
	/// Gets the subscriber name.
	/// </summary>
	string Name { get; }

	/// <summary>
	/// Subscribes the specified message type.
	/// </summary>
	/// <param name="messageType"></param>
	/// <param name="handlerType"></param>
	void Subscribe(Type messageType, Type handlerType);
}