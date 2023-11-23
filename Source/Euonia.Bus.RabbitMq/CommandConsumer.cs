using Newtonsoft.Json;
using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using Nerosoft.Euonia.Domain;

namespace Nerosoft.Euonia.Bus.RabbitMq;

/// <summary>
/// 
/// </summary>
public abstract class CommandConsumer : DisposableObject
{
    /// <summary>
    /// 
    /// </summary>
    public Action<MessageReceivedEventArgs> OnMessageReceived { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public Action<MessageAcknowledgedEventArgs> OnMessageAcknowledged { get; set; }
}

/// <summary>
/// 
/// </summary>
/// <typeparam name="TCommand"></typeparam>
public class CommandConsumer<TCommand> : CommandConsumer
    where TCommand : ICommand
{
    // ReSharper disable once StaticMemberInGenericType
    private static readonly JsonSerializerSettings _serializerSettings = new()
    {
        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
        ConstructorHandling = ConstructorHandling.Default,
        MetadataPropertyHandling = MetadataPropertyHandling.ReadAhead
    };

    private readonly IModel _channel;
    private readonly IConnection _connection;
    private readonly EventingBasicConsumer _consumer;
    private readonly IHandlerContext _handlerContext;

    /// <summary>
    /// Initializes a new instance of the <see cref="CommandConsumer{TCommand}"/> class.
    /// </summary>
    /// <param name="factory"></param>
    /// <param name="options"></param>
    /// <param name="handlerContext"></param>
    public CommandConsumer(IConnectionFactory factory, RabbitMqMessageBusOptions options, IHandlerContext handlerContext)
    {
        _handlerContext = handlerContext;
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();

        var queueName = $"{options.CommandQueueName}${typeof(TCommand).Name}$";

        _channel.QueueDeclare(queueName, true, false, false, null);

        _channel.BasicQos(0, 1, false);

        _consumer = new EventingBasicConsumer(_channel);

        _consumer.Received += HandleMessageReceived;

        _channel.BasicConsume(queueName, options.AutoAck, _consumer);
    }

    private async void HandleMessageReceived(object _, BasicDeliverEventArgs args)
    {
        var messageContext = new MessageContext();

        var body = args.Body;
        var props = args.BasicProperties;
        var replyNeeded = !string.IsNullOrEmpty(props.CorrelationId);

        var message = Deserialize(body.ToArray());
        OnMessageReceived(new MessageReceivedEventArgs(message, messageContext));

        var taskCompletion = new TaskCompletionSource<object>();
        messageContext.OnResponse += (_, a) =>
        {
            taskCompletion.TrySetResult(a.Result);
        };

        await _handlerContext.HandleAsync(message, messageContext);

        if (replyNeeded)
        {
            var result = await taskCompletion.Task;
            var replyProps = _channel.CreateBasicProperties();
            replyProps.Headers ??= new Dictionary<string, object>();
            replyProps.Headers.Add("type", result.GetType().GetFullNameWithAssemblyName());
            replyProps.CorrelationId = props.CorrelationId;

            var response = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(result, _serializerSettings));
            _channel.BasicPublish("", props.ReplyTo, replyProps, response);
            _channel.BasicAck(args.DeliveryTag, false);
        }
        else
        {
            taskCompletion.SetCanceled();
        }

        OnMessageAcknowledged(new MessageAcknowledgedEventArgs(message, messageContext));
    }

    private static TCommand Deserialize(byte[] value)
    {
        var json = Encoding.UTF8.GetString(value);
        return JsonConvert.DeserializeObject<TCommand>(json, _serializerSettings);
    }

    /// <inheritdoc />
    protected override void Dispose(bool disposing)
    {
        _consumer.Received -= HandleMessageReceived;
        _channel.Dispose();
        _connection.Dispose();
    }
}
