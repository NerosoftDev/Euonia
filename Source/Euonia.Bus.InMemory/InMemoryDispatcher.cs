﻿namespace Nerosoft.Euonia.Bus.InMemory;

/// <summary>
/// 
/// </summary>
public class InMemoryDispatcher : DisposableObject, IDispatcher
{
	/// <inheritdoc />
	public event EventHandler<MessageDispatchedEventArgs> Delivered;

	private readonly IMessenger _messenger;

	/// <summary>
	/// Initializes a new instance of the <see cref="InMemoryDispatcher"/> class.
	/// </summary>
	/// <param name="messenger"></param>
	public InMemoryDispatcher(IMessenger messenger)
	{
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

		var taskCompletion = new TaskCompletionSource();

		if (cancellationToken != default)
		{
			cancellationToken.Register(() => taskCompletion.SetCanceled(cancellationToken));
		}

		context.Completed += (_, _) =>
		{
			taskCompletion.SetResult();
		};

		_messenger.Send(pack, message.Channel);
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

		context.OnResponse += (_, args) =>
		{
			taskCompletion.SetResult((TResponse)args.Result);
		};
		context.Completed += (_, _) =>
		{
			taskCompletion.TrySetResult(default);
		};

		_messenger.Send(pack, message.Channel);
		Delivered?.Invoke(this, new MessageDispatchedEventArgs(message.Data, context));

		return await taskCompletion.Task;
	}

	/// <inheritdoc />
	protected override void Dispose(bool disposing)
	{
		_messenger.Cleanup();
	}
}