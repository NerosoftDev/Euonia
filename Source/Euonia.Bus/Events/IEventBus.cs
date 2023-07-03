namespace Nerosoft.Euonia.Bus;

/// <summary>
/// Specifies contract for event bus.
/// Implements the <see cref="IMessageBus" />
/// Implements the <see cref="IEventDispatcher" />
/// Implements the <see cref="IEventSubscriber" />
/// </summary>
/// <seealso cref="IMessageBus" />
/// <seealso cref="IEventDispatcher" />
/// <seealso cref="IEventSubscriber" />
public interface IEventBus : IMessageBus, IEventDispatcher, IEventSubscriber
{
}