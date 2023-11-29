namespace Nerosoft.Euonia.Bus;

/// <summary>
/// The options for send message.
/// </summary>
public class SendOptions : ExtendableOptions
{
	/// <summary>
	/// Gets or sets the correlation identifier.
	/// </summary>
	public string CorrelationId { get; set; }
}