using Nerosoft.Euonia.Bus;
using Nerosoft.Euonia.Claims;
using Nerosoft.Euonia.Domain;
using Nerosoft.Euonia.Validation;

namespace Nerosoft.Euonia.Application;

/// <summary>
/// Base class for application services.
/// </summary>
public abstract class BaseApplicationService : IApplicationService
{
    /// <summary>
    /// 
    /// </summary>
    public virtual ILazyServiceProvider LazyServiceProvider { get; set; }

    /// <summary>
    /// Gets the <see cref="ICommandBus"/> instance.
    /// </summary>
    protected virtual ICommandBus CommandBus => LazyServiceProvider.GetService<ICommandBus>();

    /// <summary>
    /// Gets the <see cref="IEventBus"/> instance.
    /// </summary>
    protected virtual IEventBus EventBus => LazyServiceProvider.GetService<IEventBus>();

    /// <summary>
    /// Gets the current request user principal.
    /// </summary>
    protected virtual UserPrincipal User => LazyServiceProvider.GetService<UserPrincipal>();

    /// <summary>
    /// Gets the default command handle timeout.
    /// </summary>
    protected virtual TimeSpan CommandTimeout => TimeSpan.FromSeconds(300);

    /// <summary>
    /// Send command message of <typeparamref name="TCommand"/> using <see cref="CommandBus"/>.
    /// </summary>
    /// <param name="command"></param>
    /// <param name="responseHandler"></param>
    /// <param name="cancellationToken"></param>
    /// <typeparam name="TCommand"></typeparam>
    protected virtual async Task SendCommandAsync<TCommand>(TCommand command, Action<CommandResponse> responseHandler, CancellationToken cancellationToken = default)
        where TCommand : class, ICommand
    {
        if (cancellationToken == default)
        {
            cancellationToken = new CancellationTokenSource(CommandTimeout).Token;
        }

        Validator.Validate(command);

        await CommandBus.SendAsync(command, responseHandler, cancellationToken);
    }

    /// <summary>
    /// Sends a command asynchronously and returns the response.
    /// </summary>
    /// <typeparam name="TCommand">The type of command being sent.</typeparam>
    /// <param name="command">The command being sent.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A <see cref="CommandResponse"/> object.</returns>
    protected virtual async Task<CommandResponse> SendCommandAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default)
        where TCommand : class, ICommand
    {
        if (cancellationToken == default)
        {
            cancellationToken = new CancellationTokenSource(CommandTimeout).Token;
        }

        Validator.Validate(command);

        return await CommandBus.SendAsync<TCommand, CommandResponse>(command, cancellationToken);
    }

    /// <summary>
    /// Sends a command asynchronously with a typed response.
    /// </summary>
    /// <typeparam name="TCommand">The type of the command to send.</typeparam>
    /// <typeparam name="TResult">The type of the response.</typeparam>
    /// <param name="command">The command to send.</param>
    /// <param name="responseHandler">The handler for the response object.</param>
    /// <param name="cancellationToken">The optional cancellation token.</param>
    protected virtual async Task SendCommandAsync<TCommand, TResult>(TCommand command, Action<CommandResponse<TResult>> responseHandler, CancellationToken cancellationToken = default)
        where TCommand : class, ICommand
    {
        if (cancellationToken == default)
        {
            cancellationToken = new CancellationTokenSource(CommandTimeout).Token;
        }

        Validator.Validate(command);

        await CommandBus.SendAsync(command, responseHandler, cancellationToken);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="command"></param>
    /// <param name="cancellationToken"></param>
    /// <typeparam name="TCommand"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    /// <returns></returns>
    protected virtual async Task<CommandResponse<TResult>> SendCommandAsync<TCommand, TResult>(TCommand command, CancellationToken cancellationToken = default)
        where TCommand : class, ICommand
    {
        if (cancellationToken == default)
        {
            cancellationToken = new CancellationTokenSource(CommandTimeout).Token;
        }

        Validator.Validate(command);

        return await CommandBus.SendAsync<TCommand, CommandResponse<TResult>>(command, cancellationToken);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="context"></param>
    /// <param name="responseHandler"></param>
    /// <param name="cancellationToken"></param>
    /// <typeparam name="TCommand"></typeparam>
    protected virtual async Task SendCommandAsync<TCommand>(PipelineCommand<TCommand> context, Action<CommandResponse> responseHandler, CancellationToken cancellationToken = default)
        where TCommand : class, ICommand
    {
        var response = await context.Pipeline.RunAsync(context.Command, async command => await SendCommandAsync((TCommand)command, cancellationToken));
        responseHandler?.Invoke(response);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="context"></param>
    /// <param name="responseHandler"></param>
    /// <param name="cancellationToken"></param>
    /// <typeparam name="TCommand"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    protected virtual async Task SendCommandAsync<TCommand, TResult>(PipelineCommand<TCommand> context, Action<CommandResponse<TResult>> responseHandler, CancellationToken cancellationToken = default)
        where TCommand : class, ICommand
    {
        var response = await SendCommandAsync<TCommand, TResult>(context, cancellationToken);
        responseHandler?.Invoke(response);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="context"></param>
    /// <param name="cancellationToken"></param>
    /// <typeparam name="TCommand"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    /// <returns></returns>
    protected virtual async Task<CommandResponse<TResult>> SendCommandAsync<TCommand, TResult>(PipelineCommand<TCommand> context, CancellationToken cancellationToken = default)
        where TCommand : class, ICommand
    {
        var response = await context.Pipeline.RunAsync(context.Command, async command => await SendCommandAsync<TCommand, TResult>((TCommand)command, cancellationToken));
        return (CommandResponse<TResult>)response;
    }

    /// <summary>
    /// Publish application event message using <see cref="EventBus"/>.
    /// </summary>
    /// <param name="event"></param>
    /// <typeparam name="TEvent"></typeparam>
    protected async void PublishEvent<TEvent>(TEvent @event)
        where TEvent : class, IEvent
    {
        if (EventBus == null)
        {
            return;
        }

        await EventBus.PublishAsync(@event);
    }

    /// <summary>
    /// Publish application event message using <see cref="EventBus"/> with specified name.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="event"></param>
    /// <typeparam name="TEvent"></typeparam>
    protected async void PublishEvent<TEvent>(string name, TEvent @event)
        where TEvent : class
    {
        if (EventBus == null)
        {
            return;
        }

        await EventBus.PublishAsync(name, @event);
    }
}