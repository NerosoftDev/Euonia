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

	private readonly IHandlerContext _handler;

	/// <summary>
	/// Initializes a new instance of the <see cref="InMemorySubscriber"/> class.
	/// </summary>
	/// <param name="handler"></param>
	public InMemorySubscriber(IHandlerContext handler)
	{
		_handler = handler;
	}

	/// <inheritdoc />
	public string Name { get; } = nameof(InMemorySubscriber);

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
		await _handler.HandleAsync(pack.Message.Data, pack.Context, pack.Aborted);
		MessageAcknowledged?.Invoke(this, new MessageAcknowledgedEventArgs(pack.Message, pack.Context));
	}
}