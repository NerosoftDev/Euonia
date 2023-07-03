using System.Reflection;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nerosoft.Euonia.Domain;
using RabbitMQ.Client;

namespace Nerosoft.Euonia.Bus.RabbitMq;

/// <summary>
/// The command bus implement using RabbitMQ.
/// </summary>
public class CommandBus : MessageBus, ICommandBus
{
    private readonly ConnectionFactory _factory;
    private readonly ILogger<CommandBus> _logger;
    private bool _disposed;

    private readonly Dictionary<Type, CommandConsumer> _consumers = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="CommandBus"/>.
    /// </summary>
    /// <param name="handlerContext"></param>
    /// <param name="monitor"></param>
    /// <param name="accessor"></param>
    /// <param name="logger"></param>
    public CommandBus(IMessageHandlerContext handlerContext, IOptionsMonitor<RabbitMqMessageBusOptions> monitor, IServiceAccessor accessor, ILoggerFactory logger)
        : base(handlerContext, monitor, accessor)
    {
        _logger = logger.CreateLogger<CommandBus>();
        _factory = new ConnectionFactory { Uri = new Uri(Options.Connection) };

        HandlerContext.MessageSubscribed += HandleMessageSubscribed;
    }

    /// <summary>
    /// Gets the message mediator.
    /// </summary>
    private IMediator Mediator => ServiceAccessor.GetService<IMediator>();

    private void HandleMessageSubscribed(object sender, MessageSubscribedEventArgs args)
    {
        if (args.MessageType.GetCustomAttribute<DistributedCommandAttribute>() == null)
        {
            return;
        }

        var consumerType = typeof(CommandConsumer<>).MakeGenericType(args.MessageType);
        var consumer = (CommandConsumer)Activator.CreateInstance(consumerType, _factory, Options, HandlerContext);
        if (consumer == null)
        {
            throw new InvalidOperationException($"Could not create consumer for message type {args.MessageType.FullName}");
        }

        consumer.OnMessageAcknowledged = OnMessageAcknowledged;
        consumer.OnMessageReceived = OnMessageReceived;
        _consumers.Add(args.MessageType, consumer);

        OnMessageSubscribed(args);
    }

    /// <inheritdoc />
    public async Task SendAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default)
        where TCommand : ICommand
    {
        if (typeof(TCommand).GetCustomAttribute<DistributedCommandAttribute>() != null)
        {
            await SendCommandAsync(command, cancellationToken);
        }
        else
        {
            var request = new CommandRequest<TCommand, object>(command, false);
            await Mediator.Send(request, cancellationToken);
        }
    }

    /// <inheritdoc />
    public async Task<TResult> SendAsync<TCommand, TResult>(TCommand command, CancellationToken cancellationToken = default)
        where TCommand : ICommand
    {
        TResult result;

        if (typeof(TCommand).GetCustomAttribute<DistributedCommandAttribute>() != null)
        {
            result = await SendCommandAsync<TResult>(command, cancellationToken).ConfigureAwait(false);
        }
        else
        {
            var request = new CommandRequest<TCommand, object>(command, true);
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
    public async Task<TResult> SendAsync<TResult>(ICommand<TResult> request, CancellationToken cancellationToken = default)
    {
        return await Mediator.Send(request, cancellationToken);
    }

    /// <inheritdoc />
    public void Subscribe<TCommand, THandler>()
        where TCommand : ICommand
        where THandler : ICommandHandler<TCommand>
    {
        HandlerContext.Register<TCommand, THandler>();
    }

    // ReSharper disable once SuggestBaseTypeForParameter
    private async Task<TResult> SendCommandAsync<TResult>(ICommand command, CancellationToken cancellationToken = default)
    {
        var type = command.GetType().Name;

        var messageBody = Serialize(command);
        using (var client = new CommandClient(Options, _logger))
        {
            try
            {
                var result = await client.CallAsync<TResult>(messageBody, type, cancellationToken);

                OnMessageDispatched(new MessageDispatchedEventArgs(command, null));

                return result;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error: {Message}", exception.Message);
                throw;
            }
        }
    }

    // ReSharper disable once SuggestBaseTypeForParameter
    private async Task SendCommandAsync(ICommand command, CancellationToken cancellationToken = default)
    {
        var type = command.GetType().Name;

        var messageBody = Serialize(command);
        using (var client = new CommandClient(Options, _logger))
        {
            try
            {
                await client.CallAsync(messageBody, type, cancellationToken);

                OnMessageDispatched(new MessageDispatchedEventArgs(command, null));
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error: {Message}", exception.Message);
                throw;
            }
        }
    }

    /// <summary>
    /// Releases unmanaged and - optionally - managed resources.
    /// </summary>
    /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
    protected override void Dispose(bool disposing)
    {
        if (_disposed)
        {
            return;
        }

        if (disposing)
        {
            foreach (var (_, consumer) in _consumers)
            {
                consumer.Dispose();
            }
        }

        _disposed = true;
    }
}
