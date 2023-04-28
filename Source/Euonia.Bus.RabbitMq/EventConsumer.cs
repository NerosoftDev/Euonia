using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using Nerosoft.Euonia.Domain;

namespace Nerosoft.Euonia.Bus.RabbitMq;

/// <summary>
/// The event consumer
/// </summary>
public class EventConsumer : DisposableObject
{
    private readonly IModel _channel;
    private readonly IConnection _connection;
    private readonly EventingBasicConsumer _consumer;
    private readonly string _messageName;
    private readonly RabbitMqMessageBusOptions _options;
    private readonly IMessageHandlerContext _handlerContext;

    internal EventConsumer(IConnectionFactory factory, RabbitMqMessageBusOptions options, IMessageHandlerContext handlerContext, string messageName)
    {
        _options = options;
        _handlerContext = handlerContext;
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
        _messageName = messageName;

        string queueName;

        if (string.IsNullOrWhiteSpace(options.EventQueueName))
        {
            _channel.ExchangeDeclare(messageName, options.ExchangeType);
            queueName = _channel.QueueDeclare().QueueName;
        }
        else
        {
            _channel.QueueDeclare(options.EventQueueName, true, false, false, null);
            queueName = options.EventQueueName;
        }

        _channel.QueueBind(queueName, messageName, options.RoutingKey ?? "*");

        _consumer = new EventingBasicConsumer(_channel);

        _consumer.Received += HandleMessageReceived;

        _channel.BasicConsume(string.Empty, options.AutoAck, _consumer);
    }

    private async void HandleMessageReceived(object sender, BasicDeliverEventArgs args)
    {
        var body = Encoding.UTF8.GetString(args.Body.ToArray());

        var @event = new NamedEvent(_messageName, body);
        OnMessageReceived(new MessageReceivedEventArgs(@event, null));

        var context = new MessageContext();

        await _handlerContext.HandleAsync(@event, context);

        if (!_options.AutoAck)
        {
            _channel.BasicAck(args.DeliveryTag, false);
        }

        OnMessageAcknowledged(new MessageAcknowledgedEventArgs(@event, null));
    }

    protected override void Dispose(bool disposing)
    {
        _consumer.Received -= HandleMessageReceived;
        _channel.Dispose();
        _connection.Dispose();
    }

    /// <summary>
    /// 
    /// </summary>
    public Action<MessageReceivedEventArgs> OnMessageReceived { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public Action<MessageAcknowledgedEventArgs> OnMessageAcknowledged { get; set; }
}
