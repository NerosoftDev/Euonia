using System.Reflection;
using MediatR;
using Nerosoft.Euonia.Domain;

namespace Nerosoft.Euonia.Bus.InMemory;

/// <summary>
/// Class CommandBus.
/// Implements the <see cref="MessageBus" />
/// Implements the <see cref="ICommandBus" />
/// </summary>
/// <seealso cref="MessageBus" />
/// <seealso cref="ICommandBus" />
public class CommandBus : MessageBus, ICommandBus
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CommandBus"/> class.
    /// </summary>
    /// <param name="handlerContext">The message handler context.</param>
    /// <param name="accessor"></param>
    public CommandBus(IMessageHandlerContext handlerContext, IServiceAccessor accessor)
        : base(handlerContext, accessor)
    {
        MessageReceived += HandleMessageReceivedEvent;

        HandlerContext.MessageSubscribed += HandleMessageSubscribedEvent;
    }

    /// <summary>
    /// 
    /// </summary>
    private IMediator Mediator => ServiceAccessor.GetService<IMediator>();
    
    private void HandleMessageSubscribedEvent(object sender, MessageSubscribedEventArgs args)
    {
        OnMessageSubscribed(args);
    }

    /// <summary>
    /// Sends the asynchronous.
    /// </summary>
    /// <typeparam name="TCommand">The type of the t command.</typeparam>
    /// <param name="command">The command.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>Task.</returns>
    public async Task SendAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default)
        where TCommand : ICommand
    {
        if (typeof(TCommand).GetCustomAttribute<DistributedCommandAttribute>() != null)
        {
            var context = new MessageContext(command);
            using (context)
            {
                await SendCommandAsync(command, context, cancellationToken);
            }
        }
        else
        {
            var request = new CommandRequest<TCommand, object>(command, false);
            await Mediator.Send(request, cancellationToken);
        }
    }

    /// <summary>
    /// Subscribes this instance.
    /// </summary>
    /// <typeparam name="TCommand">The type of the t command.</typeparam>
    /// <typeparam name="THandler">The type of the t handler.</typeparam>
    public void Subscribe<TCommand, THandler>()
        where TCommand : ICommand
        where THandler : ICommandHandler<TCommand>
    {
        HandlerContext.Register<TCommand, THandler>();
    }

    /// <summary>
    /// Sends the command.
    /// </summary>
    /// <param name="message">The message.</param>
    /// <param name="messageContext">The message context.</param>
    /// <param name="cancellationToken"></param>
    private async Task SendCommandAsync(IMessage message, MessageContext messageContext, CancellationToken cancellationToken = default)
    {
        await Task.Run(() =>
        {
            MessageQueue.GetQueue(message.GetType()).Enqueue(message, messageContext, MessageProcessType.Send);
            OnMessageDispatched(new MessageDispatchedEventArgs(message, messageContext));
        }, cancellationToken);
    }

    /// <summary>
    /// send as an asynchronous operation.
    /// </summary>
    /// <typeparam name="TResult">The type of the t result.</typeparam>
    /// <typeparam name="TCommand">The type of the t command.</typeparam>
    /// <param name="command">The command.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>Task&lt;TResult&gt;.</returns>
    public async Task<TResult> SendAsync<TCommand, TResult>(TCommand command, CancellationToken cancellationToken = default)
        where TCommand : ICommand
    {
        TResult result;

        if (typeof(TCommand).GetCustomAttribute<DistributedCommandAttribute>() != null)
        {
            // See https://stackoverflow.com/questions/18760252/timeout-an-async-method-implemented-with-taskcompletionsource
            var taskCompletion = new TaskCompletionSource<TResult>();

            if (cancellationToken != default)
            {
                cancellationToken.Register(() => taskCompletion.TrySetCanceled(), false);
            }

            var messageContext = new MessageContext(command);
            messageContext.Replied += (_, args) =>
            {
                taskCompletion.TrySetResult((TResult)args.Result);
            };

            messageContext.Completed += (_, _) =>
            {
                taskCompletion.TrySetResult(default);
            };

            await SendCommandAsync(command, messageContext, cancellationToken);

            result = await taskCompletion.Task;
        }
        else
        {
            var request = new CommandRequest<TCommand, object>(command, true);
            System.Diagnostics.Debug.WriteLine(Mediator.GetHashCode());
            result = await Mediator.Send(request, cancellationToken).ContinueWith(task => (TResult)task.Result, cancellationToken);
        }

        return result;
    }

    /// <inheritdoc />
    public async Task SendAsync<TCommand, TResult>(TCommand command, Action<TResult> callback, CancellationToken cancellationToken = default)
        where TCommand : ICommand
    {
        var result = await SendAsync<TCommand, TResult>(command, cancellationToken);
        callback.Invoke(result);
    }

    /// <inheritdoc />
    public async Task<TResult> SendAsync<TResult>(ICommand<TResult> command, CancellationToken cancellationToken = default)
    {
        var requestType = typeof(CommandRequest<,>).MakeGenericType(command.GetType(), typeof(object));
        var request = Activator.CreateInstance(requestType, command);
        return await Mediator.Send(request!, cancellationToken).ContinueWith(task => (TResult)task.Result, cancellationToken);
    }

    /// <inheritdoc />
    protected override void Dispose(bool disposing)
    {
    }

    private async void HandleMessageReceivedEvent(object sender, MessageReceivedEventArgs args)
    {
        OnMessageAcknowledged(new MessageAcknowledgedEventArgs(args.Message, args.MessageContext));
        await HandlerContext.HandleAsync(args.Message, args.MessageContext);
    }
}