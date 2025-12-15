namespace Nerosoft.Euonia.Bus;

/// <summary>
/// Defines a strategy for dispatching messages.
/// </summary>
public interface ITransportStrategy
{
	/// <summary>
	/// The name of the strategy.
	/// </summary>
	string Name { get; }

	/// <summary>
	/// Determines whether the specified message type can be dispatched by the transport.
	/// </summary>
	/// <param name="messageType"></param>
	/// <returns></returns>
	bool Outbound(Type messageType);

	/// <summary>
	/// Determines whether the specified message type can be received by the transport.
	/// </summary>
	/// <param name="messageType"></param>
	/// <returns></returns>
	bool Inbound(Type messageType);
	//void Dispatch(object message, IEnumerable<Func<object, Task>> handlers);
}