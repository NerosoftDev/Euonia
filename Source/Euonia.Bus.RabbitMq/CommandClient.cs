using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Polly;
using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using Microsoft.Extensions.Logging;

namespace Nerosoft.Euonia.Bus.RabbitMq;

/// <summary>
/// The command message handle client.
/// </summary>
public class CommandClient : DisposableObject
{
    private readonly RabbitMqMessageBusOptions _options;
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly string _replyQueueName;
    private readonly EventingBasicConsumer _consumer;
    private bool _disposed;
    private readonly ILogger _logger;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="options"></param>
    /// <param name="logger"></param>
    public CommandClient(RabbitMqMessageBusOptions options, ILogger logger)
    {
        _options = options;
        _logger = logger;
        var factory = new ConnectionFactory { Uri = new Uri(options.Connection) };

        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
        _replyQueueName = _channel.QueueDeclare().QueueName;
        _consumer = new EventingBasicConsumer(_channel);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="message"></param>
    /// <param name="type"></param>
    /// <param name="cancellationToken"></param>
    /// <typeparam name="TResult"></typeparam>
    /// <returns></returns>
    public async Task<TResult> CallAsync<TResult>(byte[] message, string type, CancellationToken cancellationToken = default)
    {
        var task = new TaskCompletionSource<TResult>();

        _consumer.Received += (_, args) =>
        {
            _logger.LogInformation("Callback message received: {CorrelationId}", args.BasicProperties.CorrelationId);
            var body = args.Body.ToArray();
            try
            {
                var content = Encoding.UTF8.GetString(body);
                var settings = new JsonSerializerSettings();
                var response = JsonConvert.DeserializeObject<TResult>(content, settings);

                task.TrySetResult(response);
            }
            catch (Exception exception)
            {
                _logger.LogError("Error deserializing response: {Exception}", exception);
                task.TrySetException(exception);
            }
        };

        var props = _channel.CreateBasicProperties();
        props.Headers ??= new Dictionary<string, object>();
        props.Headers[Constants.MessageHeaderCommandType] = type;
        var correlationId = Guid.NewGuid().ToString();
        props.CorrelationId = correlationId;
        props.ReplyTo = _replyQueueName;

        try
        {
            Policy.Handle<Exception>()
                  .WaitAndRetry(_options.MaxFailureRetries, _ => TimeSpan.FromSeconds(1), (exception, _, retryCount, _) =>
                  {
                      _logger.LogError(exception, "Retry:{RetryCount}, {Message}", retryCount, exception.Message);
                  })
                  .Execute(() =>
                  {
                      _channel.BasicPublish("", $"{_options.CommandQueueName}${type}$", props, message);
                      _channel.BasicConsume(_consumer, _replyQueueName, true);
                  });
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Message publish failed:{Message}", exception.Message);
            throw;
        }

        cancellationToken.Register(() => task.TrySetCanceled(), false);

        return await task.Task;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="message"></param>
    /// <param name="type"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task CallAsync(byte[] message, string type, CancellationToken cancellationToken = default)
    {
        await Task.Run(() =>
        {
            try
            {
                var props = _channel.CreateBasicProperties();
                props.Headers ??= new Dictionary<string, object>();
                props.Headers[Constants.MessageHeaderCommandType] = type;

                Policy.Handle<Exception>()
                      .WaitAndRetry(_options.MaxFailureRetries, _ => TimeSpan.FromSeconds(1), (exception, _, retryCount, _) =>
                      {
                          _logger.LogError(exception, "Retry:{RetryCount}, {Message}", retryCount, exception.Message);
                      })
                      .Execute(() =>
                      {
                          _channel.BasicPublish("", $"{_options.CommandQueueName}${type}$", props, message);
                      });
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Message publish failed:{Message}", exception.Message);
                throw;
            }
        }, cancellationToken);
    }

    /// <inheritdoc />
    protected override void Dispose(bool disposing)
    {
        if (_disposed)
        {
            return;
        }

        if (disposing)
        {
            _channel?.Dispose();
            _connection?.Dispose();
        }

        _disposed = true;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="json"></param>
    /// <param name="path"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    protected virtual Exception GetException(string json, string path, Type type)
    {
        var jsonObject = JObject.Parse(json);
        var message = jsonObject.GetValue(path)?.ToString();
        if (message == null)
        {
            return null;
        }

        return JsonConvert.DeserializeObject(message, type) as Exception;
    }
}