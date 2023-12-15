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
		await HandleAsync(pack.Message.Channel, pack.Message.Data, pack.Context, pack.Aborted);
		MessageAcknowledged?.Invoke(this, new MessageAcknowledgedEventArgs(pack.Message, pack.Context));
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="channel"></param>
	/// <param name="message"></param>
	/// <param name="context"></param>
	/// <param name="cancellationToken"></param>
	/// <returns></returns>
	protected abstract Task HandleAsync(string channel, object message, MessageContext context, CancellationToken cancellationToken = default);
}