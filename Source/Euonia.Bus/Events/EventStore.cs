using Nerosoft.Euonia.Domain;

namespace Nerosoft.Euonia.Bus;

/// <summary>
/// The abstract implement of <see cref="IEventStore" />.
/// Implements the <see cref="IEventStore" />
/// </summary>
/// <seealso cref="IEventStore" />
public abstract class EventStore : IEventStore
{
    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    /// <inheritdoc />
    public abstract void Dispose();

    /// <summary>
    /// Loads the events from event store, by using the specified originator CLR type, originator identifier and the sequence values.
    /// </summary>
    /// <typeparam name="TKey">The type of the originator key.</typeparam>
    /// <param name="originatorType">Type of the originator CLR type.</param>
    /// <param name="originatorId">The originator identifier.</param>
    /// <param name="sequenceMin">The minimum event sequence value (inclusive).</param>
    /// <param name="sequenceMax">The maximum event sequence value (inclusive).</param>
    /// <returns>The events.</returns>
    /// <inheritdoc />
    public IEnumerable<IEvent> Load<TKey>(string originatorType, TKey originatorId, long sequenceMin = Constants.MinimalSequence, long sequenceMax = Constants.MaximumSequence)
        where TKey : IEquatable<TKey>
    {
        var aggregates = LoadAggregates(originatorType, originatorId, sequenceMin, sequenceMax);
        foreach (var aggregate in aggregates)
        {
            if (aggregate.EventPayload is IEvent @event)
            {
                yield return @event;
            }
        }
    }

    /// <summary>
    /// load as an asynchronous operation.
    /// </summary>
    /// <typeparam name="TKey">The type of the originator key.</typeparam>
    /// <param name="originatorType">Type of the originator CLR type.</param>
    /// <param name="originatorId">The originator identifier.</param>
    /// <param name="sequenceMin">The minimum event sequence value (inclusive).</param>
    /// <param name="sequenceMax">The maximum event sequence value (inclusive).</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The events.</returns>
    /// <inheritdoc />
    public async Task<IEnumerable<IEvent>> LoadAsync<TKey>(string originatorType, TKey originatorId, long sequenceMin = Constants.MinimalSequence, long sequenceMax = Constants.MaximumSequence, CancellationToken cancellationToken = default)
        where TKey : IEquatable<TKey>
    {
        var aggregates = await LoadAggregatesAsync(originatorType, originatorId, sequenceMin, sequenceMax, cancellationToken);
        var events = new List<IEvent>();
        foreach (var aggregate in aggregates)
        {
            if (aggregate.EventPayload is IEvent @event)
            {
                events.Add(@event);
            }
        }

        return events;
    }

    /// <summary>
    /// Saves the specified event to the current event store.
    /// </summary>
    /// <param name="event">The event to be saved.</param>
    public void Save(IEvent @event)
    {
        var aggregate = @event.GetEventAggregate();
        SaveAggregate(aggregate);
    }

    /// <summary>
    /// Saves the specified events to the current event store.
    /// </summary>
    /// <param name="events">The events to be saved.</param>
    public void Save(IEnumerable<IEvent> events)
    {
        var aggregates = new List<EventAggregate>();
        aggregates.AddRange(events.Select(e => e.GetEventAggregate()));
        SaveAggregates(aggregates);
    }

    /// <summary>
    /// save as an asynchronous operation.
    /// </summary>
    /// <param name="event">The event to be saved.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>Task.</returns>
    /// <inheritdoc />
    public async Task SaveAsync(IEvent @event, CancellationToken cancellationToken = default)
    {
        var aggregate = @event.GetEventAggregate();
        await SaveAggregateAsync(aggregate, cancellationToken);
    }

    /// <summary>
    /// save as an asynchronous operation.
    /// </summary>
    /// <param name="events">The events to be saved.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>Task.</returns>
    /// <inheritdoc />
    public async Task SaveAsync(IEnumerable<IEvent> events, CancellationToken cancellationToken = default)
    {
        var aggregates = new List<EventAggregate>();
        aggregates.AddRange(events.Select(e => e.GetEventAggregate()));
        await SaveAggregatesAsync(aggregates, cancellationToken);
    }

    /// <summary>
    /// Loads the aggregates.
    /// </summary>
    /// <typeparam name="TKey">The type of the t key.</typeparam>
    /// <param name="originatorType">Type of the originator color.</param>
    /// <param name="originatorId">The originator identifier.</param>
    /// <param name="sequenceMin">The sequence minimum.</param>
    /// <param name="sequenceMax">The sequence maximum.</param>
    /// <returns>IEnumerable&lt;EventAggregate&gt;.</returns>
    protected abstract IEnumerable<EventAggregate> LoadAggregates<TKey>(string originatorType, TKey originatorId, long sequenceMin, long sequenceMax)
        where TKey : IEquatable<TKey>;

    /// <summary>
    /// load aggregates as an asynchronous operation.
    /// </summary>
    /// <typeparam name="TKey">The type of the t key.</typeparam>
    /// <param name="originatorClrType">Type of the originator color.</param>
    /// <param name="originatorId">The originator identifier.</param>
    /// <param name="sequenceMin">The sequence minimum.</param>
    /// <param name="sequenceMax">The sequence maximum.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>Task&lt;IEnumerable&lt;EventAggregate&gt;&gt;.</returns>
    protected virtual async Task<IEnumerable<EventAggregate>> LoadAggregatesAsync<TKey>(string originatorClrType, TKey originatorId, long sequenceMin, long sequenceMax, CancellationToken cancellationToken = default)
        where TKey : IEquatable<TKey> => await Task.Run(() => LoadAggregates(originatorClrType, originatorId, sequenceMin, sequenceMax), cancellationToken);

    /// <summary>
    /// ave event aggregate.
    /// </summary>
    /// <param name="aggregate">The event aggregate to be saved.</param>
    protected abstract void SaveAggregate(EventAggregate aggregate);

    /// <summary>
    /// Save multiple event aggregates.
    /// </summary>
    /// <param name="aggregates">The event aggregates to be saved.</param>
    protected abstract void SaveAggregates(IEnumerable<EventAggregate> aggregates);

    /// <summary>
    /// Save event aggregate asynchronously.
    /// </summary>
    /// <param name="aggregate">The event aggregate to be saved.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>Task.</returns>
    protected virtual async Task SaveAggregateAsync(EventAggregate aggregate, CancellationToken cancellationToken = default)
        => await Task.Run(() => SaveAggregate(aggregate), cancellationToken);

    /// <summary>
    /// Save multiple event aggregates asynchronously.
    /// </summary>
    /// <param name="aggregates">The event aggregates to be saved.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>Task.</returns>
    protected virtual async Task SaveAggregatesAsync(IEnumerable<EventAggregate> aggregates, CancellationToken cancellationToken = default)
        => await Task.Run(() => SaveAggregates(aggregates), cancellationToken);
}