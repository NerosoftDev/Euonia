
using Microsoft.Extensions.Logging;

namespace Nerosoft.Euonia.Bus.InMemory;

/// <summary>
/// 
/// </summary>
public class InMemoryQueueConsumer : InMemoryRecipient, IQueueConsumer
{
	private readonly IHandlerContext _handler;
	private readonly ILogger<InMemoryQueueConsumer> _logger;

	/// <summary>
	/// Initializes a new instance of the <see cref="InMemoryQueueConsumer"/> class.
	/// </summary>
	/// <param name="handler"></param>
	/// <param name="logger"></param>
	public InMemoryQueueConsumer(IHandlerContext handler, ILoggerFactory logger)
	{
		_handler = handler;
		_logger = logger.CreateLogger<InMemoryQueueConsumer>();
	}

	/// <inheritdoc />
	public string Name => nameof(InMemoryQueueConsumer);

	/// <inheritdoc/>
	protected override async Task HandleAsync(string channel, object message, MessageContext context, CancellationToken cancellationToken = default)
	{
		try
		{
			await _handler.HandleAsync(channel, message, context, cancellationToken);
		}
		catch (Exception exception)
		{
			_logger.LogError(exception, "Message '{Id}' Handle Error: {Message}", context.MessageId, exception.Message);
			context.Failure(exception);
		}
		finally
		{
			context.Complete(null);
		}
	}
}