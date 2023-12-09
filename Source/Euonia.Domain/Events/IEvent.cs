namespace Nerosoft.Euonia.Domain;

/// <summary>
/// The event interface.
/// </summary>
public interface IEvent : IMessage
{
	/// <summary>
	/// Gets the event identifier.
	/// </summary>
	string EventId { get; }

	/// <summary>
	/// Override the message identifier with event identifier.
	/// </summary>
	string IMessage.MessageId => EventId;

	/// <summary>
	/// Gets or sets the sequence of the current event.
	/// </summary>
	long Sequence { get; set; }

	/// <summary>
	/// Gets the intent of the event.
	/// </summary>
	/// <returns>
	/// The intent of the event.
	/// </returns>
	string EventIntent { get; set; }

	/// <summary>
	/// Gets the .NET CLR type of the originator of the event.
	/// </summary>
	/// <returns>
	/// The .NET CLR type of the originator of the event.
	/// </returns>
	string OriginatorType { get; set; }

	/// <summary>
	/// Gets the originator identifier.
	/// </summary>
	/// <returns>
	/// The originator identifier.
	/// </returns>
	string OriginatorId { get; set; }
}