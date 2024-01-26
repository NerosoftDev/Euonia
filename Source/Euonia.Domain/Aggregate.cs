namespace Nerosoft.Euonia.Domain;

/// <summary>
/// The abstract implement of <see cref="IAggregateRoot{TKey}"/>
/// </summary>
/// <typeparam name="TKey">The identifier type.</typeparam>
public abstract class Aggregate<TKey> : Entity<TKey>, IAggregateRoot<TKey>, IHasDomainEvents
	where TKey : IEquatable<TKey>
{
	private readonly Dictionary<Type, Action<object>> _handlers = new();

	private readonly List<DomainEvent> _events = new();

	/// <summary>
	/// The events.
	/// </summary>
	public virtual IEnumerable<DomainEvent> GetEvents() => _events?.AsReadOnly();

	/// <summary>
	/// Register a handler for the specific event type.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="when"></param>
	protected virtual void Register<T>(Action<T> when)
	{
		_handlers.Add(typeof(T), @event => when((T)@event));
	}

	/// <summary>
	/// Raise up a new event.
	/// </summary>
	/// <param name="event"></param>
	public virtual void RaiseEvent<TEvent>(TEvent @event)
		where TEvent : DomainEvent
	{
		if (_handlers.TryGetValue(typeof(TEvent), out var handler))
		{
			handler(@event);
		}
		_events.Add(@event);
	}

	/// <summary>
	/// 
	/// </summary>
	/// <typeparam name="TEvent"></typeparam>
	/// <param name="event"></param>
	public virtual void Apply<TEvent>(TEvent @event)
		where TEvent : DomainEvent
	{
		if (_handlers.TryGetValue(typeof(TEvent), out var handler))
		{
			handler(@event);
		}
	}

	/// <summary>
	/// Clear events.
	/// </summary>
	public virtual void ClearEvents()
	{
		_events.Clear();
	}

	/// <summary>
	/// 
	/// </summary>
	public virtual void AttachToEvents()
	{
		foreach (var @event in _events)
		{
			@event.Attach(this);
		}
	}
}