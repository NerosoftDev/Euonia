namespace Nerosoft.Euonia.Bus.InMemory;

/// <inheritdoc cref="IEventBus" />
public class EventBus : MessageBus, IEventBus
{
    private readonly IMessageStore _messageStore;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="handlerContext"></param>
    /// <param name="accessor"></param>
    public EventBus(IHandlerContext handlerContext, IServiceAccessor accessor)
        : base(handlerContext, accessor)
    {
        MessageReceived += HandleMessageReceivedEvent;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="handlerContext"></param>
    /// <param name="accessor"></param>
    /// <param name="messageStore"></param>
    public EventBus(IHandlerContext handlerContext, IServiceAccessor accessor, IMessageStore messageStore)
        : base(handlerContext, accessor)
    {
        _messageStore = messageStore;
    }

    /// <inheritdoc />
    public async Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
        where TEvent : IEvent
    {
        if (_messageStore != null)
        {
            await _messageStore.SaveAsync(@event, cancellationToken);
        }

        await Task.Run(() =>
        {
            MessageQueue.GetQueue<TEvent>().Enqueue(@event, new MessageContext(), MessageProcessType.Dispatch);
        }, cancellationToken);

        OnMessageDispatched(new MessageDispatchedEventArgs(@event, new MessageContext()));
    }

    /// <inheritdoc />
    public void Subscribe<TEvent, THandler>() where TEvent : IEvent where THandler : IEventHandler<TEvent>
    {
        HandlerContext.Register<TEvent, THandler>();
    }

    /// <inheritdoc />
    public async Task PublishAsync<TEvent>(string name, TEvent @event, CancellationToken cancellationToken = default)
        where TEvent : class
    {
        var namedEvent = new NamedEvent(name, @event);
        if (_messageStore != null)
        {
            await _messageStore.SaveAsync(namedEvent, cancellationToken);
        }

        await Task.Run(() =>
        {
            MessageQueue.GetQueue<TEvent>().Enqueue(namedEvent, new MessageContext(), MessageProcessType.Dispatch);
        }, cancellationToken);

        OnMessageDispatched(new MessageDispatchedEventArgs(namedEvent, new MessageContext()));
    }

    /// <inheritdoc />
    protected override void Dispose(bool disposing)
    {
    }

    private async void HandleMessageReceivedEvent(object sender, MessageReceivedEventArgs args)
    {
        await HandlerContext.HandleAsync(args.Message, args.Context);

        OnMessageAcknowledged(new MessageAcknowledgedEventArgs(args.Message, args.Context));
    }
}