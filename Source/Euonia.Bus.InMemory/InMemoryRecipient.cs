namespace Nerosoft.Euonia.Bus.InMemory;

/// <summary>
/// 
/// </summary>
public abstract class InMemoryRecipient : DisposableObject, IRecipient<MessagePack>
{
	/// <summary>
	/// Occurs when [message received].
	/// </summary>
	public event EventHandler<MessageReceivedEventArgs> MessageReceived;

	/// <summary>
	/// Occurs when [message acknowledged].
	/// </summary>
	public event EventHandler<MessageAcknowledgedEventArgs> MessageAcknowledged;

	private readonly IHandlerContext _handler;

	/// <summary>
	/// Initializes a new instance of the <see cref="InMemoryRecipient"/> class.
	/// </summary>
	/// <param name="handler"></param>
	public InMemoryRecipient(IHandlerContext handler)
	{
		_handler = handler;
	}

	#region IDisposable

	/// <inheritdoc />
	protected override void Dispose(bool disposing)
	{
	}

	#endregion

	/// <inheritdoc />
	public async void Receive(MessagePack pack)
	{
		MessageReceived?.Invoke(this, new MessageReceivedEventArgs(pack.Message, pack.Context));
		await _handler.HandleAsync(pack.Message.Channel, pack.Message.Data, pack.Context, pack.Aborted);
		MessageAcknowledged?.Invoke(this, new MessageAcknowledgedEventArgs(pack.Message, pack.Context));
	}
}