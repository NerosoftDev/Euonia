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
	/// Gets or sets the sequence of the current event.
	/// </summary>
	public long Sequence { get; set; } = DateTime.UtcNow.Ticks;

	/// <summary>
	/// Attaches the current event to the specified event.
	/// </summary>
	/// <typeparam name="TKey"></typeparam>
	/// <param name="aggregate"></param>
	public void Attach<TKey>(IAggregateRoot<TKey> aggregate)
		where TKey : IEquatable<TKey>
	{
		OriginatorId = aggregate.Id.ToString();
		OriginatorType = aggregate.GetType().AssemblyQualifiedName;
		AggregatePayload = aggregate;
	}

	/// <inheritdoc/>
	/// <summary>
	/// </summary>
	/// <returns></returns>
	public override EventAggregate GetEventAggregate()
	{
		var aggregate = base.GetEventAggregate();
		aggregate.EventSequence = Sequence;
		aggregate.EventPayload = this;
		return aggregate;
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