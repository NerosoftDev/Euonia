namespace Nerosoft.Euonia.Bus;

/// <summary>
/// Represents an attribute that specifies the transports in which messages are received.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class ReceiveInAttribute : Attribute
{
	/// <summary>
	/// Initializes a new instance of the <see cref="ReceiveInAttribute"/> class.
	/// </summary>
	/// <param name="transports"></param>
	public ReceiveInAttribute(params string[] transports)
	{
		Transports = transports;
	}

	/// <summary>
	/// Gets the name of transports that receive messages.
	/// </summary>
	public IEnumerable<string> Transports { get; }
}