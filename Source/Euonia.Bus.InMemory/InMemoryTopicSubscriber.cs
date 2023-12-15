
using Microsoft.Extensions.Logging;

namespace Nerosoft.Euonia.Bus.InMemory;

/// <summary>
/// 
/// </summary>
public class InMemoryTopicSubscriber : InMemoryRecipient, ITopicSubscriber
{
	private readonly IHandlerContext _handler;
	private readonly ILogger<InMemoryTopicSubscriber> _logger;

	/// <summary>
	/// Initializes a new instance of the <see cref="InMemoryTopicSubscriber"/> class.
	/// </summary>
	/// <param name="handler"></param>
	/// <param name="logger"></param>
	public InMemoryTopicSubscriber(IHandlerContext handler, ILoggerFactory logger)
	{
		_handler = handler;
		_logger = logger.CreateLogger<InMemoryTopicSubscriber>();
	}

	/// <inheritdoc />
	public string Name => nameof(InMemoryTopicSubscriber);

	/// <inheritdoc />
	protected override async Task HandleAsync(string channel, object message, MessageContext context, CancellationToken cancellationToken = default)
	{
		try
		{
			await _handler.HandleAsync(channel, message, context, cancellationToken);
		}
		catch (Exception exception)
		{
			_logger.LogError(exception, "Message '{Id}' Handle Error: {Message}", context.MessageId, exception.Message);
		}
		finally
		{
		}
	}
}