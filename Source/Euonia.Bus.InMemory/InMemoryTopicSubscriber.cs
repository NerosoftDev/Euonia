namespace Nerosoft.Euonia.Bus.InMemory;

/// <summary>
/// 
/// </summary>
public class InMemoryTopicSubscriber : InMemoryRecipient, ITopicSubscriber
{
	/// <summary>
	/// Initializes a new instance of the <see cref="InMemoryTopicSubscriber"/> class.
	/// </summary>
	/// <param name="handler"></param>
	public InMemoryTopicSubscriber(IHandlerContext handler)
		: base(handler)
	{
	}

	/// <inheritdoc />
	public string Name => nameof(InMemoryTopicSubscriber);
}