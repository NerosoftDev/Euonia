namespace Nerosoft.Euonia.Bus.InMemory;

/// <summary>
/// 
/// </summary>
public class InMemoryDispatcher : DisposableObject, IDispatcher
{
	/// <inheritdoc />
	public event EventHandler<MessageDispatchedEventArgs> Delivered;

	/// <inheritdoc />
	public async Task PublishAsync<TMessage>(RoutedMessage<TMessage> message, CancellationToken cancellationToken = default)
		where TMessage : class
	{
		var context = new MessageContext(message);
		var pack = new MessagePack(message, context)
		{
			Aborted = cancellationToken
		};
		WeakReferenceMessenger.Default.Send(pack, message.Channel);
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

		var taskCompletion = new TaskCompletionSource();

		if (cancellationToken != default)
		{
			cancellationToken.Register(() => taskCompletion.SetCanceled(cancellationToken));
		}

		context.Failed += (_, exception) =>
		{
			taskCompletion.TrySetException(exception);
		};

		context.Completed += (_, _) =>
		{
			taskCompletion.SetResult();
		};

		StrongReferenceMessenger.Default.UnsafeSend(pack, message.Channel);

		Delivered?.Invoke(this, new MessageDispatchedEventArgs(message.Data, context));

		await taskCompletion.Task;
	}

	/// <inheritdoc />
	public async Task<TResponse> SendAsync<TMessage, TResponse>(RoutedMessage<TMessage, TResponse> message, CancellationToken cancellationToken = default)
		where TMessage : class
	{
		var context = new MessageContext(message);
		var pack = new MessagePack(message, context)
		{
			Aborted = cancellationToken
		};

		// See https://stackoverflow.com/questions/18760252/timeout-an-async-method-implemented-with-taskcompletionsource
		var taskCompletion = new TaskCompletionSource<TResponse>();
		if (cancellationToken != default)
		{
			cancellationToken.Register(() => taskCompletion.TrySetCanceled(), false);
		}

		context.Responded += (_, args) =>
		{
			taskCompletion.SetResult((TResponse)args.Result);
		};
		context.Failed += (_, exception) =>
		{
			taskCompletion.TrySetException(exception);
		};
		context.Completed += (_, _) =>
		{
			taskCompletion.TryCompleteFromCompletedTask(Task.FromResult(default(TResponse)));
		};

		StrongReferenceMessenger.Default.UnsafeSend(pack, message.Channel);
		Delivered?.Invoke(this, new MessageDispatchedEventArgs(message.Data, context));

		return await taskCompletion.Task;
	}

	/// <inheritdoc />
	protected override void Dispose(bool disposing)
	{
		StrongReferenceMessenger.Default.Reset();
		WeakReferenceMessenger.Default.Reset();
	}
}