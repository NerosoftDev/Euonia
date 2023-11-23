using System.Reflection;

namespace Nerosoft.Euonia.Bus;

/// <summary>
/// Evaluate whether a type is a message, command, or event by attribute decorated on the type.
/// </summary>
public class AttributeMessageConvention : IMessageConvention
{
	/// <inheritdoc />
	public string Name { get; } = "Attribute decoration message convention";

	/// <inheritdoc />
	public bool IsCommandType(Type type)
	{
		return type.GetCustomAttribute<CommandAttribute>(false) != null;
	}

	/// <inheritdoc />
	public bool IsEventType(Type type)
	{
		return type.GetCustomAttribute<EventAttribute>(false) != null;
	}
}