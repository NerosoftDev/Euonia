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
	public bool IsQueueType(Type messageType)
	{
		return messageType.GetCustomAttribute<QueueAttribute>(false) != null;
	}

	/// <inheritdoc />
	public bool IsTopicType(Type messageType)
	{
		return messageType.GetCustomAttribute<TopicAttribute>(false) != null;
	}

	/// <summary>
	/// Determines whether the specified type is a request.
	/// </summary>
	/// <param name="messageType"></param>
	/// <returns></returns>
	public bool IsRequestType(Type messageType)
	{
		return messageType.GetCustomAttribute<RequestAttribute>(false) != null;
	}
}