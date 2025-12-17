namespace Nerosoft.Euonia.Bus;

/// <summary>
/// Defines the contract for a transport strategy, which determines how messages
/// are handled for outgoing and incoming operations.
/// </summary>
public interface ITransportStrategy
{
	/// <summary>
	/// Gets the name of the transport strategy.
	/// </summary>
	string Name { get; }

	/// <summary>
	/// Determines whether the specified message type is allowed for outgoing operations.
	/// </summary>
	/// <param name="messageType">The type of the message to check.</param>
	/// <returns><c>true</c> if the message type is allowed for outgoing; otherwise, <c>false</c>.</returns>
	bool Outgoing(Type messageType);

	/// <summary>
	/// Determines whether the specified message type is allowed for incoming operations.
	/// </summary>
	/// <param name="messageType">The type of the message to check.</param>
	/// <returns><c>true</c> if the message type is allowed for incoming; otherwise, <c>false</c>.</returns>
	bool Incoming(Type messageType);
}