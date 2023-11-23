namespace Nerosoft.Euonia.Bus.InMemory;

/// <summary>
/// 
/// </summary>
public class InMemorySubscriber : DisposableObject, ISubscriber, IRecipient<MessagePack>
{
	/// <summary>
	/// Occurs when [message received].
	/// </summary>
	public event EventHandler<MessageReceivedEventArgs> MessageReceived;

	/// <summary>
	/// Occurs when [message acknowledged].
	/// </summary>
	public event EventHandler<MessageAcknowledgedEventArgs> MessageAcknowledged;

	/// <summary>
	/// 
	/// </summary>
	public event EventHandler<MessageSubscribedEventArgs> MessageSubscribed;

	private readonly string _channel;

	/// <summary>
	/// Initializes a new instance of the <see cref="InMemorySubscriber"/> class.
	/// </summary>
	/// <param name="channel"></param>
	public InMemorySubscriber(string channel)
	{
		_channel = channel;
	}

	/// <inheritdoc />
	public string Name { get; } = nameof(InMemorySubscriber);

	/// <inheritdoc />
	public void Subscribe(Type messageType, Type handlerType)
	{
		//throw new NotImplementedException();
	}

	#region IDisposable

	/// <inheritdoc />
	protected override void Dispose(bool disposing)
	{
	}

	#endregion

	/// <inheritdoc />
	public void Receive(MessagePack message)
	{
		MessageReceived?.Invoke(this, new MessageReceivedEventArgs(message.Message, message.Context));
	}
}