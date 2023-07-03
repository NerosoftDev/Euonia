using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Nerosoft.Euonia.Bus;

/// <inheritdoc />
public class MessageBusActiveService : BackgroundService
{
    private readonly ILogger<MessageBusActiveService> _logger;
    private readonly IServiceProvider _provider;

    /// <inheritdoc />
    public MessageBusActiveService(IServiceProvider provider, ILoggerFactory logger)
    {
        _provider = provider;
        _logger = logger.CreateLogger<MessageBusActiveService>();
    }

    /// <inheritdoc />
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogDebug("MessageBusActiveService.ExecuteAsync Called");

        ActiveCommandBus();
        ActiveEventBus();

        await Task.CompletedTask;
    }

    private void ActiveCommandBus()
    {
        try
        {
            var bus = _provider.GetService<ICommandBus>();
            if (bus != null)
            {
                bus.MessageReceived += (sender, args) =>
                {
                    _logger.LogInformation("Received command: {Id}, {MessageType}. Sender: {Sender}", args.Message.Id, args.Message.GetTypeName(), sender);
                };
            }
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Failed to resolve command bus. {Message}", exception.Message);
            throw;
        }
    }

    private void ActiveEventBus()
    {
        try
        {
            var bus = _provider.GetService<IEventBus>();
            if (bus != null)
            {
                bus.MessageReceived += (sender, args) =>
                {
                    _logger.LogInformation("Received event: {Id}, {MessageType}. Sender: {Sender}", args.Message?.Id, args.Message?.GetTypeName(), sender);
                };
            }
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Failed to resolve event bus. {Message}", exception.Message);
            throw;
        }
    }
}