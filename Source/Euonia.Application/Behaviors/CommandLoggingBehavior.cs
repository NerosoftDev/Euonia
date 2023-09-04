using Microsoft.Extensions.Logging;
using Nerosoft.Euonia.Domain;
using Nerosoft.Euonia.Pipeline;

namespace Nerosoft.Euonia.Application;

/// <summary>
/// A behavior that logs the command context and response.
/// </summary>
public class CommandLoggingBehavior : IPipelineBehavior<ICommand, CommandResponse>
{
    private readonly ILogger<CommandLoggingBehavior> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="CommandLoggingBehavior"/> class.
    /// </summary>
    /// <param name="factory">The logger service factory.</param>
    public CommandLoggingBehavior(ILoggerFactory factory)
    {
        _logger = factory.CreateLogger<CommandLoggingBehavior>();
    }

    /// <inheritdoc />
    public async Task<CommandResponse> HandleAsync(ICommand context, PipelineDelegate<ICommand, CommandResponse> next)
    {
        _logger.LogInformation("Command {Id} - {FullName}: {Context}", context.Id, context.GetType().FullName, context);
        var response = await next(context);
        _logger.LogInformation("Command {Id} - {IsSuccess} {Message}", context.Id, response.IsSuccess, response.Message);
        return response;
    }
}