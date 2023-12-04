using Microsoft.Extensions.Logging;
using Nerosoft.Euonia.Pipeline;

namespace Nerosoft.Euonia.Bus;

/// <summary>
/// 
/// </summary>
public sealed class MessageLoggingBehavior : IPipelineBehavior<IRoutedMessage>
{
	private readonly ILogger<MessageLoggingBehavior> _logger;

	/// <summary>
	/// Initializes a new instance of the <see cref="MessageLoggingBehavior"/> class.
	/// </summary>
	/// <param name="logger">The logger service factory.</param>
	public MessageLoggingBehavior(ILoggerFactory logger)
	{
		_logger = logger.CreateLogger<MessageLoggingBehavior>();
	}

	/// <inheritdoc />
	public async Task HandleAsync(IRoutedMessage context, PipelineDelegate<IRoutedMessage> next)
	{
		_logger.LogInformation("Message {Id} - {FullName}: {Context}", context.MessageId, context.GetType().FullName, context);
		await next(context);
	}
}