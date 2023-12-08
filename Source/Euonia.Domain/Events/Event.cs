namespace Nerosoft.Euonia.Domain;

/// <summary>
/// The abstract class implements <see cref="IEvent"/>.
/// </summary>
public abstract class Event : IEvent
{
	/// <summary>
	/// Initializes a new instance of the <see cref="Event"/> class.
	/// </summary>
	protected Event()
	{
		var type = GetType();
		EventIntent = type.Name;
	}

	/// <summary>
	/// Gets or sets the sequence of the current event.
	/// </summary>
	public long Sequence { get; set; } = DateTime.UtcNow.Ticks;

	/// <summary>
	/// Gets the intent of the event.
	/// </summary>
	/// <returns>The intent of the event.</returns>
	public virtual string EventIntent { get; set; }

	/// <summary>
	/// Gets the .NET CLR type of the originator of the event.
	/// </summary>
	/// <returns>The .NET CLR type of the originator of the event.</returns>
	public virtual string OriginatorType { get; set; }

	/// <summary>
	/// Gets the originator identifier.
	/// </summary>
	/// <returns>The originator identifier.</returns>
	public virtual string OriginatorId { get; set; }
}