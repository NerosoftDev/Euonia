namespace Nerosoft.Euonia.Bus;

/// <summary>
/// Defines the message convention type.
/// </summary>
public enum MessageConventionType
{
	/// <summary>
	/// None
	/// </summary>
	None,

	/// <summary>
	/// Unicast
	/// </summary>
	Unicast,

	/// <summary>
	/// Multicast
	/// </summary>
	Multicast,

	/// <summary>
	/// Request
	/// </summary>
	Request,
}