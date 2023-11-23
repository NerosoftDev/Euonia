namespace Nerosoft.Euonia.Bus.InMemory;

/// <summary>
/// 
/// </summary>
public class InMemoryBusOptions
{
	public bool LazyInitialize { get; set; } = true;

	public int MaxConcurrentCalls { get; set; } = 1;

	/// <summary>
	/// Gets or sets the messenger reference type.
	/// </summary>
	public MessengerReferenceType MessengerReference { get; set; } = MessengerReferenceType.StrongReference;
}