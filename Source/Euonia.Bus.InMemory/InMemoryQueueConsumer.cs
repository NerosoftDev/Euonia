namespace Nerosoft.Euonia.Bus.InMemory;

/// <summary>
/// 
/// </summary>
public class InMemoryQueueConsumer : InMemoryRecipient, IQueueConsumer
{
	/// <summary>
	/// Initializes a new instance of the <see cref="InMemoryQueueConsumer"/> class.
	/// </summary>
	/// <param name="handler"></param>
	public InMemoryQueueConsumer(IHandlerContext handler)
		: base(handler)
	{
	}

	/// <inheritdoc />
	public string Name => nameof(InMemoryQueueConsumer);
}