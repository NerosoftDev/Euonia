namespace Nerosoft.Euonia.Bus.InMemory;

/// <summary>
/// Defines a message pack to transport.
/// </summary>
public sealed class MessagePack
{
	/// <summary>
	/// 
	/// </summary>
	/// <param name="message"></param>
	/// <param name="context"></param>
	public MessagePack(IRoutedMessage message, IMessageContext context)
	{
		Message = message;
		Context = context;
	}

	/// <summary>
	/// Get the message.
	/// </summary>
	public IRoutedMessage Message { get; }

	/// <summary>
	/// Get the message context.
	/// </summary>
	public IMessageContext Context { get; }

	/// <summary>
	/// Gets or sets the cancellation token.
	/// </summary>
	public CancellationToken Aborted { get; set; }
}