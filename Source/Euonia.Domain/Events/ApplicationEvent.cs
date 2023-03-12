namespace Nerosoft.Euonia.Domain;

/// <summary>
/// To be added.
/// Implements the <see cref="Event" />
/// Implements the <see cref="IApplicationEvent"/>
/// </summary>
/// <seealso cref="Event" />
/// <seealso cref="IApplicationEvent" />
public abstract class ApplicationEvent : Event, IApplicationEvent
{
    /// <inheritdoc />
    public virtual long Sequence { get; set; } = DateTime.UtcNow.Ticks;
}