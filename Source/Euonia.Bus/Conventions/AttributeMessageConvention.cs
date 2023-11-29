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
	public bool IsQueueType(Type type)
	{
		return type.GetCustomAttribute<QueueAttribute>(false) != null;
	}

	/// <inheritdoc />
	public bool IsTopicType(Type type)
	{
		return type.GetCustomAttribute<TopicAttribute>(false) != null;
	}
}