using System.Reflection;

namespace Nerosoft.Euonia.Bus;

/// <summary>
/// The message subscription.
/// </summary>
internal class MessageSubscription
{
	/// <summary>
	/// Initializes a new instance of the <see cref="MessageSubscription"/> class.
	/// </summary>
	/// <param name="type"></param>
	/// <param name="handlerType"></param>
	/// <param name="handleMethod"></param>
	public MessageSubscription(Type type, Type handlerType, MethodInfo handleMethod)
		: this(type.FullName, handlerType, handleMethod)
	{
		Type = type;
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="MessageSubscription"/> class.
	/// </summary>
	/// <param name="name"></param>
	/// <param name="handlerType"></param>
	/// <param name="handleMethod"></param>
	public MessageSubscription(string name, Type handlerType, MethodInfo handleMethod)
	{
		Name = name;
		HandlerType = handlerType;
		HandleMethod = handleMethod;
	}

	/// <summary>
	/// Gets or sets the message name.
	/// </summary>
	public string Name { get; set; }

	/// <summary>
	/// Gets or sets the message type.
	/// </summary>
	public Type Type { get; set; }

	/// <summary>
	/// Gets or sets the message handler type.
	/// </summary>
	public Type HandlerType { get; set; }

	/// <summary>
	/// Gets or sets the message handler method.
	/// </summary>
	public MethodInfo HandleMethod { get; set; }
}