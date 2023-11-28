using System.Reflection;

namespace Nerosoft.Euonia.Bus;

/// <summary>
/// The message subscription.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="MessageRegistration"/> class.
/// </remarks>
/// <param name="channel"></param>
/// <param name="messageType"></param>
/// <param name="handlerType"></param>
/// <param name="method"></param>
public class MessageRegistration(string channel, Type messageType, Type handlerType, MethodInfo method)
{
	/// <summary>
	/// Gets or sets the message name.
	/// </summary>
	public string Channel { get; set; } = channel;

	/// <summary>
	/// Gets or sets the message type.
	/// </summary>
	public Type MessageType { get; set; } = messageType;

	/// <summary>
	/// Gets or sets the handler type.
	/// </summary>
	public Type HandlerType { get; set; } = handlerType;

	/// <summary>
	/// Gets or sets the handler method.
	/// </summary>
	public MethodInfo Method { get; set; } = method;
}