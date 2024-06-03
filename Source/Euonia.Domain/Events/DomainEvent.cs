namespace Nerosoft.Euonia.Domain;

/// <summary>
/// To be added.
/// Implements the <see cref="Event" />
/// Implements the <see cref="IDomainEvent" />
/// </summary>
/// <seealso cref="Event" />
/// <seealso cref="IDomainEvent" />
public abstract class DomainEvent : Event, IDomainEvent
{
	/// <summary>
	/// Attaches the current event to the specified event.
	/// </summary>
	/// <typeparam name="TKey"></typeparam>
	/// <param name="aggregate"></param>
	public void Attach<TKey>(IAggregateRoot<TKey> aggregate)
		where TKey : IEquatable<TKey>
	{
		OriginatorId = aggregate.Id?.ToString();
		OriginatorType = aggregate.GetType().AssemblyQualifiedName;
		AggregatePayload = aggregate;
	}

	/// <summary>
	/// Gets the event aggregate.
	/// </summary>
	/// <returns>EventAggregate.</returns>
	public virtual EventAggregate GetEventAggregate()
	{
		return new EventAggregate
		{
			Id = Guid.NewGuid().ToString(),
			TypeName = GetType().AssemblyQualifiedName,
			EventIntent = EventIntent,
			Timestamp = DateTime.UtcNow,
			OriginatorId = OriginatorId,
			OriginatorType = OriginatorType,
			EventSequence = Sequence,
			EventPayload = this
		};
	}

	/// <summary>
	/// Gets or sets the aggregate payload.
	/// </summary>
	/// <value>The aggregate payload.</value>
	public virtual object AggregatePayload { get; set; }

	/// <summary>
	/// Gets attached aggregate root object.
	/// </summary>
	/// <typeparam name="TAggregate"></typeparam>
	/// <returns></returns>
	public virtual TAggregate GetAggregate<TAggregate>()
		where TAggregate : IAggregateRoot
	{
		return AggregatePayload switch
		{
			null => default,
			TAggregate aggregate => aggregate,
			_ => default,
		};
	}
}