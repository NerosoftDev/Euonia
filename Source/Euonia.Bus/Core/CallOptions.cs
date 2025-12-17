namespace Nerosoft.Euonia.Bus;

/// <summary>
/// The call options.
/// </summary>
public class CallOptions : ExtendableOptions
{
	/// <summary>
	/// Gets or sets the correlation identifier.
	/// </summary>
	public string CorrelationId { get; set; }
}