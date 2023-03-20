namespace Nerosoft.Euonia.Domain;

/// <summary>
/// Interface IEventStore
/// Implements the <see cref="IDisposable" />
/// </summary>
/// <seealso cref="IDisposable" />
public interface IEventStore : IDisposable
{
    /// <summary>
    /// Save the specified event to the current event store.
    /// </summary>
    /// <param name="event">The event to be saved.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>Task.</returns>
    Task SaveAsync(IEvent @event, CancellationToken cancellationToken = default);

    /// <summary>
    /// Saves the specified events to the current event store asynchronously.
    /// </summary>
    /// <param name="events">The events to be saved.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>Task.</returns>
    Task SaveAsync(IEnumerable<IEvent> events, CancellationToken cancellationToken = default);

    /// <summary>
    /// Loads the events from event store, by using the specified originator CLR type, originator identifier and the sequence values.
    /// </summary>
    /// <typeparam name="TKey">The type of the originator key.</typeparam>
    /// <param name="originatorType">Type of the originator CLR type.</param>
    /// <param name="originatorId">The originator identifier.</param>
    /// <param name="sequenceMin">The minimum event sequence value (inclusive).</param>
    /// <param name="sequenceMax">The maximum event sequence value (inclusive).</param>
    /// <returns>The events.</returns>
    IEnumerable<IEvent> Load<TKey>(string originatorType, TKey originatorId, long sequenceMin = Constants.MinimalSequence, long sequenceMax = Constants.MaximumSequence)
        where TKey : IEquatable<TKey>;

    /// <summary>
    /// Loads the events from event store, by using the specified originator CLR type, originator identifier and the sequence values asynchronously.
    /// </summary>
    /// <typeparam name="TKey">The type of the originator key.</typeparam>
    /// <param name="originatorType">Type of the originator CLR type.</param>
    /// <param name="originatorId">The originator identifier.</param>
    /// <param name="sequenceMin">The minimum event sequence value (inclusive).</param>
    /// <param name="sequenceMax">The maximum event sequence value (inclusive).</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The events.</returns>
    Task<IEnumerable<IEvent>> LoadAsync<TKey>(string originatorType, TKey originatorId, long sequenceMin = Constants.MinimalSequence, long sequenceMax = Constants.MaximumSequence, CancellationToken cancellationToken = default)
        where TKey : IEquatable<TKey>;
}