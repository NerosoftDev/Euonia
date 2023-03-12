namespace Nerosoft.Euonia.Domain;

/// <summary>
/// The abstract class implements <see cref="IEvent"/>.
/// </summary>
public abstract class Event : Message, IEvent
{
    /// <summary>
    /// The event intent metadata key
    /// </summary>
    protected const string EventIntentMetadataKey = "$nerosoft.euonia.event.intent";

    /// <summary>
    /// The event originator type key
    /// </summary>
    protected const string EventOriginTypeKey = "$nerosoft.euonia.event.originator.type";

    /// <summary>
    /// The event originator identifier
    /// </summary>
    protected const string EventOriginatorId = "$nerosoft.euonia.event.originator.id";

    /// <summary>
    /// Initializes a new instance of the <see cref="Event"/> class.
    /// </summary>
    protected Event()
    {
        var type = GetType();
        Metadata[EventIntentMetadataKey] = type.Name;
    }

    /// <summary>
    /// Gets the intent of the event.
    /// </summary>
    /// <returns>The intent of the event.</returns>
    public virtual string GetEventIntent() => Metadata[EventIntentMetadataKey]?.ToString();

    /// <summary>
    /// Gets the .NET CLR type of the originator of the event.
    /// </summary>
    /// <returns>The .NET CLR type of the originator of the event.</returns>
    public virtual string GetOriginatorType() => Metadata[EventOriginTypeKey]?.ToString();

    /// <summary>
    /// Gets the originator identifier.
    /// </summary>
    /// <returns>The originator identifier.</returns>
    public virtual string GetOriginatorId() => Metadata[EventOriginatorId]?.ToString();

    /// <summary>
    /// Gets the event aggregate.
    /// </summary>
    /// <returns>EventAggregate.</returns>
    public virtual EventAggregate GetEventAggregate()
    {
        return new EventAggregate
        {
            Id = Guid.NewGuid(),
            TypeName = GetTypeName(),
            EventId = Id,
            EventIntent = GetEventIntent(),
            Timestamp = DateTime.UtcNow,
            OriginatorId = GetOriginatorId(),
            OriginatorType = GetOriginatorType(),
            EventPayload = this
        };
    }
}