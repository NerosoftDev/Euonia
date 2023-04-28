namespace Nerosoft.Euonia.Bus;

/// <summary>
/// Interface IMessageBus
/// Implements the <see cref="IMessageDispatcher" />
/// Implements the <see cref="IMessageSubscriber" />
/// </summary>
/// <seealso cref="IMessageDispatcher" />
/// <seealso cref="IMessageSubscriber" />
public interface IMessageBus : IMessageDispatcher, IMessageSubscriber
{
}