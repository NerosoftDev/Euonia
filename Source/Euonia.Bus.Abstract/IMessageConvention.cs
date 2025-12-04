namespace Nerosoft.Euonia.Bus;

/// <summary>
/// A set of conventions for determining if a class represents a request, queue, or topic message.
/// </summary>
public interface IMessageConvention
{
	/// <summary>
	/// The name of the convention. Used for diagnostic purposes.
	/// </summary>
	string Name { get; }

	/// <summary>
	/// Determine if a type is a command type.
	/// </summary>
	/// <param name="messageType">The type to check.</param>.
	/// <returns>true if <paramref name="messageType"/> represents a queue message.</returns>
	bool IsCommandType(Type messageType);

	/// <summary>
	/// Determine if a type is a event type.
	/// </summary>
	/// <param name="messageType">The type to check.</param>.
	/// <returns>true if <paramref name="messageType"/> represents a topic message.</returns>
	bool IsEventType(Type messageType);

	/// <summary>
	/// Determine if a type is a request type.
	/// </summary>
	/// <param name="messageType">The type to check.</param>
	/// <returns>true if <paramref name="messageType"/> represents a request message.</returns>
	bool IsRequestType(Type messageType);
}