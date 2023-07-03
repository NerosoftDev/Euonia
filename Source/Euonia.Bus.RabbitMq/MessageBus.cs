using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Nerosoft.Euonia.Bus.RabbitMq;

public abstract class MessageBus : DisposableObject, IMessageBus
{
    private static readonly JsonSerializerSettings _serializerSettings = new()
    {
        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
        ConstructorHandling = ConstructorHandling.Default,
        MetadataPropertyHandling = MetadataPropertyHandling.ReadAhead
    };

    /// <inheritdoc />
    public event EventHandler<MessageSubscribedEventArgs> MessageSubscribed;

    /// <inheritdoc />
    public event EventHandler<MessageDispatchedEventArgs> MessageDispatched;

    /// <inheritdoc />
    public event EventHandler<MessageReceivedEventArgs> MessageReceived;

    /// <inheritdoc />
    public event EventHandler<MessageAcknowledgedEventArgs> MessageAcknowledged;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="handlerContext"></param>
    /// <param name="monitor"></param>
    /// <param name="accessor"></param>
    internal MessageBus(IMessageHandlerContext handlerContext, IOptionsMonitor<RabbitMqMessageBusOptions> monitor, IServiceAccessor accessor)
    {
        HandlerContext = handlerContext;
        ServiceAccessor = accessor;
        Options = monitor.CurrentValue;
        monitor.OnChange((options, _) =>
        {
            Options.Connection = options.Connection;
            Options.AutoAck = options.AutoAck;
            Options.ExchangeName = options.ExchangeName;
            Options.ExchangeType = options.ExchangeType;
            Options.CommandQueueName = options.CommandQueueName;
            Options.MaxFailureRetries = options.MaxFailureRetries;
        });
    }

    /// <summary>
    /// Gets the service accessor.
    /// </summary>
    protected IServiceAccessor ServiceAccessor { get; }

    /// <summary>
    /// Gets the message handler context.
    /// </summary>
    protected IMessageHandlerContext HandlerContext { get; }

    /// <summary>
    /// Gets the message bus options.
    /// </summary>
    protected RabbitMqMessageBusOptions Options { get; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="args"></param>
    protected virtual void OnMessageSubscribed(MessageSubscribedEventArgs args)
    {
        MessageSubscribed?.Invoke(this, args);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="args"></param>
    protected virtual void OnMessageDispatched(MessageDispatchedEventArgs args)
    {
        MessageDispatched?.Invoke(this, args);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="args"></param>
    protected virtual void OnMessageReceived(MessageReceivedEventArgs args)
    {
        MessageReceived?.Invoke(this, args);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="args"></param>
    protected virtual void OnMessageAcknowledged(MessageAcknowledgedEventArgs args)
    {
        MessageAcknowledged?.Invoke(this, args);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    protected virtual byte[] Serialize(object message)
    {
        if (message == null)
        {
            return Array.Empty<byte>();
        }

        var type = message.GetType();

        var stringValue = type.IsClass ? JsonConvert.SerializeObject(message, _serializerSettings) : message.ToString();
        return string.IsNullOrEmpty(stringValue) ? Array.Empty<byte>() : Encoding.UTF8.GetBytes(stringValue);
    }

    protected virtual string GetHeaderValue(IDictionary<string, object> header, string key)
    {
        if (header == null)
        {
            return string.Empty;
        }

        if (header.TryGetValue(key, out var value))
        {
            return value switch
            {
                null => string.Empty,
                string @string => @string,
                byte[] bytes => Encoding.UTF8.GetString(bytes),
                _ => value.ToString()
            };
        }

        return string.Empty;
    }

    public virtual void Subscribe(Type messageType, Type handlerType)
    {
        HandlerContext.Register(messageType, handlerType);
    }

    public virtual void Subscribe(string messageName, Type handlerType)
    {
        HandlerContext.Register(messageName, handlerType);
    }
}
