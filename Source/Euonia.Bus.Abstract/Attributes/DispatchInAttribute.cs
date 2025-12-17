namespace Nerosoft.Euonia.Bus;

/// <summary>
/// Represents an attribute that specifies the transports in which messages are dispatched.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class DispatchInAttribute : Attribute
{
	/// <summary>
	/// Initializes a new instance of the <see cref="DispatchInAttribute"/> class.
	/// </summary>
	/// <param name="transports"></param>
	public DispatchInAttribute(params string[] transports)
	{
		Transports = transports;
	}

	/// <summary>
	/// Gets the name of transports that dispatch messagess.
	/// </summary>
	public IEnumerable<string> Transports { get; }
}