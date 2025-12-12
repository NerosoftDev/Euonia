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
	public bool IsUnicastType(Type messageType)
	{
		return messageType.GetCustomAttribute<CommandAttribute>(false) != null;
	}

	/// <inheritdoc />
	public bool IsMulticastType(Type messageType)
	{
		return messageType.GetCustomAttribute<EventAttribute>(false) != null;
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