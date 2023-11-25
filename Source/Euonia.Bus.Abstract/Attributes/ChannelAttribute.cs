namespace Nerosoft.Euonia.Bus;

/// <summary>
/// Represents the attributed event has a specified name.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class ChannelAttribute : Attribute
{
	/// <summary>
	/// Gets the event name.
	/// </summary>
	public string Name { get; }

	/// <summary>
	/// Initialize a new instance of <see cref="ChannelAttribute"/>.
	/// </summary>
	/// <param name="name"></param>
	/// <exception cref="ArgumentNullException"></exception>
	public ChannelAttribute(string name)
	{
		if (string.IsNullOrWhiteSpace(name))
		{
			throw new ArgumentNullException(nameof(name));
		}

		Name = name;
	}
}