using MediatR;
using Nerosoft.Euonia.Domain;

namespace Nerosoft.Euonia.Bus;

/// <summary>
/// Specifies contract for command handler.
/// Implements the <see cref="IMessageHandler" />
/// </summary>
/// <seealso cref="IMessageHandler" />
public interface ICommandHandler : IMessageHandler
{
}

/// <summary>
/// Specifies contract for command handler.
/// Implements the <see cref="IMessageHandler{TCommand}" />
/// Implements the <see cref="ICommandHandler" />
/// </summary>
/// <typeparam name="TCommand">The type of command.</typeparam>
/// <seealso cref="IMessageHandler{TCommand}" />
/// <seealso cref="ICommandHandler" />
public interface ICommandHandler<TCommand> : IMessageHandler<TCommand>, ICommandHandler, IRequestHandler<CommandRequest<TCommand, object>, object>
    where TCommand : ICommand
{
    async Task<object> IRequestHandler<CommandRequest<TCommand, object>, object>.Handle(CommandRequest<TCommand, object> request, CancellationToken cancellationToken)
    {
        var messageContext = new MessageContext(request.Command);

        var taskCompletion = new TaskCompletionSource<object>();

        if (request.WaitResponse)
        {
            messageContext.Replied += (_, args) =>
            {
                taskCompletion.TrySetResult(args.Result);
            };
        }

        messageContext.Completed += (_, _) =>
        {
            taskCompletion.TrySetResult(null);
        };

        using (messageContext)
        {
            await HandleAsync(request.Command, messageContext, cancellationToken);
        }

        return await taskCompletion.Task;
    }
}

/// <summary>
/// 
/// </summary>
/// <typeparam name="TCommand"></typeparam>
/// <typeparam name="TResult"></typeparam>
public interface ICommandHandler<TCommand, TResult> : IMessageHandler<TCommand>, ICommandHandler, IRequestHandler<CommandRequest<TCommand, TResult>, TResult>
    where TCommand : ICommand<TResult>
{
    async Task<TResult> IRequestHandler<CommandRequest<TCommand, TResult>, TResult>.Handle(CommandRequest<TCommand, TResult> request, CancellationToken cancellationToken)
    {
        var taskCompletion = new TaskCompletionSource<TResult>();

        if (cancellationToken != default)
        {
            cancellationToken.Register(() => taskCompletion.TrySetCanceled(), false);
        }

        var messageContext = new MessageContext();
        messageContext.Replied += (_, args) =>
        {
            var result = (TResult)args.Result;
            taskCompletion.TrySetResult(result);
        };

        await HandleAsync(request.Command, messageContext, cancellationToken);
        return await taskCompletion.Task;
    }
}