using Nerosoft.Euonia.Domain;

namespace Nerosoft.Euonia.Bus;

/// <summary>
/// Specifies contract for command sender.
/// Implements the <see cref="IMessageDispatcher" />
/// </summary>
/// <seealso cref="IMessageDispatcher" />
public interface ICommandSender : IMessageDispatcher
{
    /// <summary>
    /// Asynchronously send a command request to a single handler
    /// </summary>
    /// <typeparam name="TCommand">The type of command.</typeparam>
    /// <param name="command">The command to be sent.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>Task.</returns>
    Task SendAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default)
        where TCommand : ICommand;

    /// <summary>
    /// Asynchronously send a command request to a single handler
    /// </summary>
    /// <typeparam name="TResult">The type of send result.</typeparam>
    /// <typeparam name="TCommand">The type of command.</typeparam>
    /// <param name="command">The command to be sent.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>Task&lt;TResult&gt;.</returns>
    Task<TResult> SendAsync<TCommand, TResult>(TCommand command, CancellationToken cancellationToken = default)
        where TCommand : ICommand;

    /// <summary>
    /// Asynchronously send a command request to a single handler
    /// </summary>
    /// <typeparam name="TResult">The type of send result.</typeparam>
    /// <typeparam name="TCommand">The type of command.</typeparam>
    /// <param name="command">The command to be sent.</param>
    /// <param name="callback">The action to execute after command has been sent.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>Task.</returns>
    Task SendAsync<TCommand, TResult>(TCommand command, Action<TResult> callback, CancellationToken cancellationToken = default)
        where TCommand : ICommand;

    /// <summary>
    /// Asynchronously send a command request to a single handler
    /// </summary>
    /// <typeparam name="TResult">Response type</typeparam>
    /// <param name="command">Request object</param>
    /// <param name="cancellationToken">Optional cancellation token</param>
    /// <returns>A task that represents the send operation. The task result contains the handler response</returns>
    Task<TResult> SendAsync<TResult>(ICommand<TResult> command, CancellationToken cancellationToken = default);
}