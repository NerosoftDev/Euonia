using Nerosoft.Euonia.Domain;

namespace Nerosoft.Euonia.Bus;

/// <summary>
/// Specifies contract for command subscriber.
/// Implements the <see cref="IMessageSubscriber" />
/// </summary>
/// <seealso cref="IMessageSubscriber" />
public interface ICommandSubscriber : IMessageSubscriber
{
    /// <summary>
    /// Subscribe a command.
    /// </summary>
    /// <typeparam name="TCommand">The type of command.</typeparam>
    /// <typeparam name="THandler">The type of command handler.</typeparam>
    void Subscribe<TCommand, THandler>()
        where TCommand : ICommand
        where THandler : ICommandHandler<TCommand>;
}