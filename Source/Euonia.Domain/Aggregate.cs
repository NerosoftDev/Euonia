namespace Nerosoft.Euonia.Domain;

/// <summary>
/// The abstract implement of <see cref="IAggregateRoot{TKey}"/>
/// </summary>
/// <typeparam name="TKey">The identifier type.</typeparam>
public abstract class Aggregate<TKey> : Entity<TKey>, IAggregateRoot<TKey>, IHasDomainEvents
    where TKey : IEquatable<TKey>
{
    private readonly List<DomainEvent> _events = new();

    /// <summary>
    /// The events.
    /// </summary>
    public IEnumerable<DomainEvent> GetEvents() => _events?.AsReadOnly();

    /// <summary>
    /// Raise up a new event.
    /// </summary>
    /// <param name="event"></param>
    public virtual void RaiseEvent<TEvent>(TEvent @event)
        where TEvent : DomainEvent
    {
        _events.Add(@event);
    }

    /// <summary>
    /// Remove event.
    /// </summary>
    /// <param name="event"></param>
    public virtual void RemoveEvent<TEvent>(TEvent @event)
        where TEvent : DomainEvent
    {
        _events.Remove(@event);
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