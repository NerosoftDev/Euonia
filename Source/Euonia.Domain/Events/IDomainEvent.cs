namespace Nerosoft.Euonia.Domain;

/// <summary>
/// Interface IDomainEvent
/// Implements the <see cref="IEvent" />
/// </summary>
/// <seealso cref="IEvent" />
public interface IDomainEvent : IEvent
{
	/// <summary>
	/// Gets or sets the sequence of the current event.
	/// </summary>
	long Sequence { get; set; }

	/// <summary>
	/// Attaches the current event to the specified event.
	/// </summary>
	/// <param name="aggregate"></param>
	/// <typeparam name="TKey"></typeparam>
	void Attach<TKey>(IAggregateRoot<TKey> aggregate) where TKey : IEquatable<TKey>;
}