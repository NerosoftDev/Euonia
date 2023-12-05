using System.Reflection;

namespace Nerosoft.Euonia.Bus;

/// <summary>
/// The message subscription.
/// </summary>
public class MessageRegistration
{
	/// <summary>
	/// Initializes a new instance of the &lt;see cref="MessageRegistration"/&gt; class.
	/// </summary>
	/// <param name="channel"></param>
	/// <param name="messageType"></param>
	/// <param name="handlerType"></param>
	/// <param name="method"></param>
	public MessageRegistration(string channel, Type messageType, Type handlerType, MethodInfo method)
	{
		Channel = channel;
		MessageType = messageType;
		HandlerType = handlerType;
		Method = method;
	}

	/// <summary>
	/// Gets or sets the message name.
	/// </summary>
	public string Channel { get; }

	/// <summary>
	/// Gets or sets the message type.
	/// </summary>
	public Type MessageType { get; }

	/// <summary>
	/// Gets or sets the handler type.
	/// </summary>
	public Type HandlerType { get; } 

	/// <summary>
	/// Gets or sets the handler method.
	/// </summary>
	public MethodInfo Method { get; }
}