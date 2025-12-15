using System.Reflection;

namespace Nerosoft.Euonia.Bus;

/// <summary>
/// Evaluate whether a type is marked as a local message.
/// </summary>
public class LocalMessageTransportStrategy : ITransportStrategy
{
	public string Name { get; } = "Local message transport strategy";

	/// <summary>
	/// Initializes a new instance of the <see cref="LocalMessageTransportStrategy"/> class.
	/// </summary>
	/// <param name="messageType"></param>
	/// <returns></returns>
	public bool Outbound(Type messageType)
	{
		return messageType.GetCustomAttribute<LocalMessageAttribute>() != null;
	}

	/// <summary>
	/// Determines whether the specified message type can be received by the transport.
	/// </summary>
	/// <param name="messageType"></param>
	/// <returns></returns>
	public bool Inbound(Type messageType)
	{
		return messageType.GetCustomAttribute<LocalMessageAttribute>() != null;
	}
}