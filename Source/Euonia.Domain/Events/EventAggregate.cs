namespace Nerosoft.Euonia.Domain;

/// <summary>
/// The event aggregate root.
/// </summary>
public class EventAggregate : IAggregateRoot<Guid>
{
    public object[] GetKeys()
    {
        return new object[] { Id };
    }

    /// <summary>
    /// Gets or sets the identifier of current instance.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets then event identifier.
    /// </summary>
    public Guid EventId { get; set; }

    /// <summary>
    /// Gets or sets the timestamp.
    /// </summary>
    public DateTime Timestamp { get; set; }

    /// <summary>
    /// Gets or sets the type name.
    /// </summary>
    public string TypeName { get; set; }

    /// <summary>
    /// Gets or sets the event intent.
    /// </summary>
    public string EventIntent { get; set; }

    /// <summary>
    /// Gets or sets the originator type.
    /// </summary>
    public string OriginatorType { get; set; }

    /// <summary>
    /// Gets or sets the originator identifier.
    /// </summary>
    public string OriginatorId { get; set; }

    /// <summary>
    /// Gets or sets the event payload.
    /// </summary>
    public object EventPayload { get; set; }

    /// <summary>
    /// Gets or sets the event sequence.
    /// </summary>
    public long EventSequence { get; set; }

    /// <summary>
    /// Returns a <see cref="System.String" /> that represents this instance.
    /// </summary>
    /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
    public override string ToString() => EventIntent;
}