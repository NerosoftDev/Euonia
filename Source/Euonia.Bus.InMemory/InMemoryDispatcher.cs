using Microsoft.Extensions.Options;

namespace Nerosoft.Euonia.Bus.InMemory;

/// <summary>
/// 
/// </summary>
public class InMemoryDispatcher : DisposableObject, IDispatcher
{
	/// <inheritdoc />
	public event EventHandler<MessageDispatchedEventArgs> Delivered;

	private readonly InMemoryBusOptions _options;

	private readonly IMessenger _messenger;

	/// <summary>
	/// Initializes a new instance of the <see cref="InMemoryDispatcher"/> class.
	/// </summary>
	/// <param name="messenger"></param>
	/// <param name="options"></param>
	public InMemoryDispatcher(IMessenger messenger, IOptions<InMemoryBusOptions> options)
	{
		_options = options.Value;
		_messenger = messenger;
	}

	/// <inheritdoc />
	public async Task PublishAsync<TMessage>(RoutedMessage<TMessage> message, CancellationToken cancellationToken = default)
		where TMessage : class
	{
		var context = new MessageContext(message);
		var pack = new MessagePack(message, context)
		{
			Aborted = cancellationToken
		};
		_messenger.Send(pack, message.Channel);
		Delivered?.Invoke(this, new MessageDispatchedEventArgs(message.Data, context));
		await Task.CompletedTask;
	}

	/// <inheritdoc />
	public async Task SendAsync<TMessage>(RoutedMessage<TMessage> message, CancellationToken cancellationToken = default)
		where TMessage : class
	{
		var context = new MessageContext(message);
		var pack = new MessagePack(message, context)
		{
			Aborted = cancellationToken
		};

		var taskCompletionSource = new TaskCompletionSource();

		context.Completed += (_, _) =>
		{
			taskCompletionSource.SetResult();
		};

		_messenger.Send(pack, message.Channel);
		Delivered?.Invoke(this, new MessageDispatchedEventArgs(message.Data, context));

		await taskCompletionSource.Task;
	}

	/// <inheritdoc />
	public async Task<TResult> SendAsync<TMessage, TResult>(RoutedMessage<TMessage> message, CancellationToken cancellationToken = default)
		where TMessage : class
	{
		var context = new MessageContext(message);
		var pack = new MessagePack(message, context)
		{
			Aborted = cancellationToken
		};

		var taskCompletionSource = new TaskCompletionSource<TResult>();
		context.OnResponse += (_, args) =>
		{
			taskCompletionSource.SetResult((TResult)args.Result);
		};
		context.Completed += (_, _) =>
		{
			taskCompletionSource.SetResult(default);
		};

		_messenger.Send(pack, message.Channel);
		Delivered?.Invoke(this, new MessageDispatchedEventArgs(message.Data, context));

		return await taskCompletionSource.Task;
	}

	/// <inheritdoc />
	protected override void Dispose(bool disposing)
	{
		_messenger.Cleanup();
	}
}