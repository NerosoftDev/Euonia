using System.Reflection;

namespace Nerosoft.Euonia.Bus;

/// <summary>
/// Evaluate whether a type is marked as a distributed message.
/// </summary>
public class DistributedMessageTransportStrategy : ITransportStrategy
{
	/// <summary>
	/// The name of the strategy.
	/// </summary>
	public string Name => "Distributed message transport strategy";

	/// <summary>
	/// Determines whether the specified message type can be dispatched by the transport.
	/// </summary>
	/// <param name="messageType"></param>
	/// <returns></returns>
	public bool Outbound(Type messageType)
	{
		return messageType.GetCustomAttribute<DistributedMessageAttribute>() != null;
	}

	/// <summary>
	/// Determines whether the specified message type can be received by the transport.
	/// </summary>
	/// <param name="messageType"></param>
	/// <returns></returns>
	public bool Inbound(Type messageType)
	{
		return messageType.GetCustomAttribute<DistributedMessageAttribute>() != null;
	}
}