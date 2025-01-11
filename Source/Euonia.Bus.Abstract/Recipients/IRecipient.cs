namespace Nerosoft.Euonia.Bus;

/// <summary>
/// Defines a recipient of messages.
/// </summary>
public interface IRecipient : IDisposable
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
	/// Gets the subscriber name.
	/// </summary>
	string Name { get; }
}