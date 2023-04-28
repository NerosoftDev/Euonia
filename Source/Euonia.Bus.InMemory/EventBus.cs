using Nerosoft.Euonia.Domain;

namespace Nerosoft.Euonia.Bus.InMemory;

/// <inheritdoc cref="IEventBus" />
public class EventBus : MessageBus, IEventBus
{
    private readonly IEventStore _eventStore;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="handlerContext"></param>
    /// <param name="accessor"></param>
    public EventBus(IMessageHandlerContext handlerContext, IServiceAccessor accessor)
        : base(handlerContext, accessor)
    {
        MessageReceived += HandleMessageReceivedEvent;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="handlerContext"></param>
    /// <param name="accessor"></param>
    /// <param name="eventStore"></param>
    public EventBus(IMessageHandlerContext handlerContext, IServiceAccessor accessor, IEventStore eventStore)
        : base(handlerContext, accessor)
    {
        _eventStore = eventStore;
    }

    /// <inheritdoc />
    public async Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
        where TEvent : IEvent
    {
        if (_eventStore != null)
        {
            await _eventStore.SaveAsync(@event, cancellationToken);
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
        if (_eventStore != null)
        {
            await _eventStore.SaveAsync(namedEvent, cancellationToken);
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
        await HandlerContext.HandleAsync(args.Message, args.MessageContext);

        OnMessageAcknowledged(new MessageAcknowledgedEventArgs(args.Message, args.MessageContext));
    }
}