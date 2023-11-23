namespace Nerosoft.Euonia.Bus.InMemory;

/// <summary>
/// Class MessageBus.
/// Implements the <see cref="DisposableObject" />
/// Implements the <see cref="IBus" />
/// </summary>
/// <seealso cref="DisposableObject" />
public abstract class MessageBus : DisposableObject
{
	/// <summary>
	/// Occurs when [message subscribed].
	/// </summary>
	public event EventHandler<MessageSubscribedEventArgs> MessageSubscribed;

	/// <summary>
	/// Occurs when [message dispatched].
	/// </summary>
	public event EventHandler<MessageDispatchedEventArgs> Dispatched;

	/// <summary>
	/// Occurs when [message received].
	/// </summary>
	public event EventHandler<MessageReceivedEventArgs> MessageReceived;

	/// <summary>
	/// Occurs when [message acknowledged].
	/// </summary>
	public event EventHandler<MessageAcknowledgedEventArgs> MessageAcknowledged;

	/// <summary>
	/// Initializes a new instance of the <see cref="MessageBus"/> class.
	/// </summary>
	/// <param name="handlerContext">The message handler context.</param>
	/// <param name="accessor"></param>
	protected MessageBus(IHandlerContext handlerContext, IServiceAccessor accessor)
	{
		HandlerContext = handlerContext;
		ServiceAccessor = accessor;
	}

	/// <summary>
	/// Gets the service accessor.
	/// </summary>
	protected IServiceAccessor ServiceAccessor { get; }

	/// <summary>
	/// Gets the message handler context.
	/// </summary>
	/// <value>The message handler context.</value>
	protected IHandlerContext HandlerContext { get; }

	/// <summary>
	/// Handles the <see cref="E:MessageSubscribed" /> event.
	/// </summary>
	/// <param name="args">The <see cref="MessageSubscribedEventArgs"/> instance containing the event data.</param>
	protected virtual void OnMessageSubscribed(MessageSubscribedEventArgs args)
	{
		var queue = new MessageQueue();
		queue.MessagePushed += (sender, e) =>
		{
			var message = (sender as MessageQueue)?.Dequeue();
			if (message == null)
			{
				return;
			}

			OnMessageReceived(new MessageReceivedEventArgs(message, e.Context));
		};
		MessageQueue.AddQueue(args.MessageType, queue);
		MessageSubscribed?.Invoke(this, args);
	}

	/// <summary>
	/// Handles the <see cref="E:Delivered" /> event.
	/// </summary>
	/// <param name="args">The <see cref="MessageDispatchedEventArgs"/> instance containing the event data.</param>
	protected virtual void OnMessageDispatched(MessageDispatchedEventArgs args)
	{
		Dispatched?.Invoke(this, args);
	}

	/// <summary>
	/// Handles the <see cref="E:MessageReceived" /> event.
	/// </summary>
	/// <param name="args">The <see cref="MessageReceivedEventArgs"/> instance containing the event data.</param>
	protected virtual void OnMessageReceived(MessageReceivedEventArgs args)
	{
		MessageReceived?.Invoke(this, args);
	}

	/// <summary>
	/// Handles the <see cref="E:MessageAcknowledged" /> event.
	/// </summary>
	/// <param name="args">The <see cref="MessageAcknowledgedEventArgs"/> instance containing the event data.</param>
	protected virtual void OnMessageAcknowledged(MessageAcknowledgedEventArgs args)
	{
		MessageAcknowledged?.Invoke(this, args);
	}

	/// <summary>
	/// Subscribes the specified message type.
	/// </summary>
	/// <param name="messageType"></param>
	/// <param name="handlerType"></param>
	public virtual void Subscribe(Type messageType, Type handlerType)
	{
		HandlerContext.Register(messageType, handlerType);
	}

	/// <summary>
	/// Subscribes the specified message name.
	/// </summary>
	/// <param name="messageName"></param>
	/// <param name="handlerType"></param>
	public virtual void Subscribe(string messageName, Type handlerType)
	{
		HandlerContext.Register(messageName, handlerType);
	}
}