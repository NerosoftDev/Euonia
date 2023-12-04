namespace Nerosoft.Euonia.Bus;

/// <summary>
/// Represents the attributed method would handle an message.
/// </summary>
[AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
public class SubscribeAttribute : Attribute
{
	/// <summary>
	/// Initializes a new instance of the <see cref="SubscribeAttribute"/> class.
	/// </summary>
	/// <param name="name"></param>
	public SubscribeAttribute(string name)
	{
		Name = name;
	}

	/// <summary>
	/// Gets the name of the message.
	/// </summary>
	public string Name { get; }

	/// <summary>
	/// Gets or sets the message group name.
	/// </summary>
	public string Group { get; set; }
}