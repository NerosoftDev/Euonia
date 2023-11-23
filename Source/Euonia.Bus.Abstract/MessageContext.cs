namespace Nerosoft.Euonia.Bus;

/// <summary>
/// The message context.
/// </summary>
public sealed class MessageContext : IMessageContext
{
	private readonly WeakEventManager _events = new();

	private readonly IDictionary<string, string> _headers = new Dictionary<string, string>();

	private bool _disposedValue;

	/// <summary>
	/// Initializes a new instance of the <see cref="MessageContext"/> class.
	/// </summary>
	public MessageContext()
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="MessageContext"/> class.
	/// </summary>
	/// <param name="message"></param>
	public MessageContext(object message)
	{
		Message = message;
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="MessageContext"/> class.
	/// </summary>
	/// <param name="pack"></param>
	public MessageContext(IRoutedMessage pack)
		: this(pack.Data)
	{
		MessageId = pack.MessageId;
		CorrelationId = pack.CorrelationId;
		ConversationId = pack.ConversationId;
		RequestTraceId = pack.RequestTraceId;
	}

	/// <summary>
	/// Invoked while message was handled and replied to dispatcher.
	/// </summary>
	public event EventHandler<MessageRepliedEventArgs> OnResponse
	{
		add => _events.AddEventHandler(value);
		remove => _events.RemoveEventHandler(value);
	}

	/// <summary>
	/// Invoke while message context disposed.
	/// </summary>
	public event EventHandler<MessageHandledEventArgs> Completed
	{
		add => _events.AddEventHandler(value);
		remove => _events.RemoveEventHandler(value);
	}

	/// <inheritdoc />
	public object Message { get; }

	/// <inheritdoc />
	public string MessageId { get; set; }

	/// <inheritdoc />
	public string CorrelationId { get; set; }

	/// <inheritdoc />
	public string ConversationId { get; set; }

	/// <inheritdoc />
	public string RequestTraceId { get; set; }

	/// <inheritdoc />
	public IReadOnlyDictionary<string, string> Headers { get; set; }

	/// <summary>
	/// Replies message handling result to message dispatcher.
	/// </summary>
	/// <param name="message">The message to reply.</param>
	public void Response(object message)
	{
		_events.HandleEvent(this, new MessageRepliedEventArgs(message), nameof(OnResponse));
	}

	/// <summary>
	/// Replies message handling result to message dispatcher.
	/// </summary>
	/// <typeparam name="TMessage">The type of the message.</typeparam>
	/// <param name="message">The message to reply.</param>
	public void Response<TMessage>(TMessage message)
	{
		Response((object)message);
	}

	/// <summary>
	/// Called after the message has been handled.
	/// This operate will raised up the <see cref="Completed"/> event.
	/// </summary>
	/// <param name="message"></param>
	public void Complete(object message)
	{
		_events.HandleEvent(this, new MessageHandledEventArgs(message), nameof(Completed));
	}

	/// <summary>
	/// Called after the message has been handled.
	/// This operate will raised up the <see cref="Completed"/> event.
	/// </summary>
	/// <param name="message"></param>
	/// <param name="handlerType"></param>
	public void Complete(object message, Type handlerType)
	{
		_events.HandleEvent(this, new MessageHandledEventArgs(message) { HandlerType = handlerType }, nameof(Completed));
	}

	/// <summary>
	/// Called after the message has been handled.
	/// </summary>
	/// <param name="disposing"></param>
	private void Dispose(bool disposing)
	{
		if (_disposedValue)
		{
			return;
		}

		if (disposing)
		{
			Complete(Message);
		}

		_events.RemoveEventHandlers();
		_disposedValue = true;
	}

	/// <summary>
	/// Finalizes the current instance of the <see cref="MessageContext"/> class.
	/// </summary>
	~MessageContext()
	{
		Dispose(disposing: false);
	}

	/// <inheritdoc />
	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}
}