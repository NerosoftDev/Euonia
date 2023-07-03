namespace Nerosoft.Euonia.Domain;

/// <summary>
/// The event interface.
/// </summary>
public interface IEvent : IMessage
{
    /// <summary>
    /// Gets the intent of the event.
    /// </summary>
    /// <returns>
    /// The intent of the event.
    /// </returns>
    string GetEventIntent();

    /// <summary>
    /// Gets the .NET CLR type of the originator of the event.
    /// </summary>
    /// <returns>
    /// The .NET CLR type of the originator of the event.
    /// </returns>
    string GetOriginatorType();

    /// <summary>
    /// Gets the originator identifier.
    /// </summary>
    /// <returns>
    /// The originator identifier.
    /// </returns>
    string GetOriginatorId();

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    EventAggregate GetEventAggregate();
}