namespace Nerosoft.Euonia.Domain;

/// <summary>
/// Represent the event was raised by application service.
/// </summary>
public interface IApplicationEvent : IEvent
{
    /// <summary>
    /// Gets or sets the sequence of the current event.
    /// </summary>
    long Sequence { get; set; }
}