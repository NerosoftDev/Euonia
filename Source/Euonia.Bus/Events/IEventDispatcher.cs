using Nerosoft.Euonia.Domain;

namespace Nerosoft.Euonia.Bus;

/// <summary>
/// Interface IEventDispatcher
/// Implements the <see cref="IMessageDispatcher" />
/// </summary>
/// <seealso cref="IMessageDispatcher" />
public interface IEventDispatcher : IMessageDispatcher
{
    /// <summary>
    /// Publish event to event bus asynchronously.
    /// </summary>
    /// <typeparam name="TEvent">The type of the t event.</typeparam>
    /// <param name="event">The event.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>Task.</returns>
    Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
        where TEvent : IEvent;

    /// <summary>
    /// Publish event with given name.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="event"></param>
    /// <param name="cancellationToken"></param>
    /// <typeparam name="TEvent"></typeparam>
    /// <returns></returns>
    Task PublishAsync<TEvent>(string name, TEvent @event, CancellationToken cancellationToken = default)
        where TEvent : class;
}