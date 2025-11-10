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
	/// Determine if a type is a queue type.
	/// </summary>
	/// <param name="type">The type to check.</param>.
	/// <returns>true if <paramref name="type"/> represents a queue message.</returns>
	bool IsQueueType(Type type);

	/// <summary>
	/// Determine if a type is a topic type.
	/// </summary>
	/// <param name="type">The type to check.</param>.
	/// <returns>true if <paramref name="type"/> represents a topic message.</returns>
	bool IsTopicType(Type type);
	
	/// <summary>
	/// Determine if a type is a request type.
	/// </summary>
	/// <param name="type">The type to check.</param>
	/// <returns>true if <paramref name="type"/> represents a request message.</returns>
	bool IsRequestType(Type type);
}