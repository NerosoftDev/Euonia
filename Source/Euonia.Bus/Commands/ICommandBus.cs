namespace Nerosoft.Euonia.Bus;

/// <summary>
/// Specifies contract for command bus.
/// Implements the <see cref="IMessageBus" />
/// Implements the <see cref="ICommandSender" />
/// Implements the <see cref="ICommandSubscriber" />
/// </summary>
/// <seealso cref="IMessageBus" />
/// <seealso cref="ICommandSender" />
/// <seealso cref="ICommandSubscriber" />
public interface ICommandBus : IMessageBus, ICommandSender, ICommandSubscriber
{
}