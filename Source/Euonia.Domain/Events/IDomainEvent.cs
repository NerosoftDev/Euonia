namespace Nerosoft.Euonia.Domain;

/// <summary>
/// Interface IDomainEvent
/// Implements the <see cref="IEvent" />
/// </summary>
/// <seealso cref="IEvent" />
public interface IDomainEvent : IEvent
{
	/// <summary>
	/// Attaches the current event to the specified event.
	/// </summary>
	/// <param name="aggregate"></param>
	/// <typeparam name="TKey"></typeparam>
	void Attach<TKey>(IAggregateRoot<TKey> aggregate)
		where TKey : IEquatable<TKey>;

	/// <summary>
	/// 
	/// </summary>
	/// <returns></returns>
	EventAggregate GetEventAggregate();
}