using Nerosoft.Euonia.Domain;

namespace Nerosoft.Euonia.Bus;

/// <summary>
/// Interface IEventSubscriber
/// Implements the <see cref="IMessageSubscriber" />
/// </summary>
/// <seealso cref="IMessageSubscriber" />
public interface IEventSubscriber : IMessageSubscriber
{
    /// <summary>
    /// Subscribes this instance.
    /// </summary>
    /// <typeparam name="TEvent">The type of the t event.</typeparam>
    /// <typeparam name="THandler">The type of the t handler.</typeparam>
    void Subscribe<TEvent, THandler>()
        where TEvent : IEvent
        where THandler : IEventHandler<TEvent>;
}