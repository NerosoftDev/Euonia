using System.Net.Sockets;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Polly;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;

namespace Nerosoft.Euonia.Bus.RabbitMq;

/// <summary>
/// Defines MQ transport use RabbitMQ.
/// </summary>
public sealed class RabbitMqTransport : ITransport
{
	/// <inheritdoc />
	public event EventHandler<MessageDeliveredEventArgs> Delivered;

	/// <inheritdoc />
	public string Identifier { get; set; }

	private readonly RabbitMqMessageBusOptions _options;
	private readonly IPersistentConnection _connection;
	private readonly ILogger<RabbitMqDispatcher> _logger;

	/// <summary>
	/// Initialize a new instance of <see cref="RabbitMqDispatcher"/>.
	/// </summary>
	/// <param name="connection"></param>
	/// <param name="options"></param>
	/// <param name="logger"></param>
	public RabbitMqTransport(IPersistentConnection connection, RabbitMqMessageBusOptions options, ILoggerFactory logger)
	{
		_logger = logger.CreateLogger<RabbitMqDispatcher>();
		_connection = connection;
		_options = options;
	}

	/// <inheritdoc />
	public async Task PublishAsync<TMessage>(RoutedMessage<TMessage> message, CancellationToken cancellationToken = default)
		where TMessage : class
	{
		using var channel = await _connection.CreateChannelAsync();

		var typeName = message.GetTypeName();

		var props = new BasicProperties();
		props.Headers ??= new Dictionary<string, object>();
		props.Headers[Constants.MessageHeaders.MessageType] = typeName;
		props.Type = typeName;

		await Policy.Handle<SocketException>()
		            .Or<BrokerUnreachableException>()
		            .WaitAndRetryAsync(_options.MaxFailureRetries, _ => TimeSpan.FromSeconds(3), (exception, _, retryCount, _) =>
		            {
			            _logger.LogError(exception, "Retry:{RetryCount}, {Message}", retryCount, exception.Message);
		            })
		            .ExecuteAsync(async () =>
		            {
			            var messageBody = await SerializeAsync(message, cancellationToken);

			            await channel.ExchangeDeclareAsync(_options.ExchangeName, _options.ExchangeType, cancellationToken: cancellationToken);
			            await channel.BasicPublishAsync(_options.ExchangeName, $"{_options.TopicName}${message.Channel}$", true, props, messageBody, cancellationToken: cancellationToken);

			            Delivered?.Invoke(this, new MessageDeliveredEventArgs(message.Data, null));
		            });
	}

	/// <inheritdoc />
	public async Task SendAsync<TMessage>(RoutedMessage<TMessage> message, CancellationToken cancellationToken = default) where TMessage : class
	{
		var task = new TaskCompletionSource<dynamic>();

		var requestQueueName = $"{_options.QueueName}${message.Channel}$";

		using var channel = await _connection.CreateChannelAsync();

		await CheckQueueAsync(channel, requestQueueName);

		var responseQueueName = await channel.ResponseQueueDeclareAsync(cancellationToken);
		var consumer = new AsyncEventingBasicConsumer(channel);

		consumer.ReceivedAsync += OnReceivedAsync;

		var typeName = message.GetTypeName();

		var props = new BasicProperties();
		props.Headers ??= new Dictionary<string, object>();
		props.Headers[Constants.MessageHeaders.MessageType] = typeName;
		props.Type = typeName;
		props.CorrelationId = message.CorrelationId;
		props.ReplyTo = responseQueueName;

		await Policy.Handle<SocketException>()
		            .Or<BrokerUnreachableException>()
		            .WaitAndRetryAsync(_options.MaxFailureRetries, _ => TimeSpan.FromSeconds(1), (exception, _, retryCount, _) =>
		            {
			            _logger.LogError(exception, "Retry:{RetryCount}, {Message}", retryCount, exception.Message);
		            })
		            .ExecuteAsync(async () =>
		            {
			            var messageBody = await SerializeAsync(message, cancellationToken);
			            await channel.BasicPublishAsync("", requestQueueName, true, props, messageBody, cancellationToken);
			            await channel.BasicConsumeAsync(responseQueueName, true, consumer, cancellationToken: cancellationToken);

			            Delivered?.Invoke(this, new MessageDeliveredEventArgs(message.Data, null));
		            });

		await task.Task;
		consumer.ReceivedAsync -= OnReceivedAsync;
		return;

		async Task OnReceivedAsync(object sender, BasicDeliverEventArgs args)
		{
			if (args.BasicProperties.CorrelationId != message.CorrelationId)
			{
				return;
			}

			var body = args.Body.ToArray();
			var response = JsonConvert.DeserializeObject<RabbitMqReply<object>>(Encoding.UTF8.GetString(body), Constants.SerializerSettings);
			if (response.IsSuccess)
			{
				task.SetResult(response.Result);
			}
			else
			{
				task.SetException(response.Error);
			}

			await Task.CompletedTask;
		}
	}

	/// <inheritdoc />
	public async Task<TResponse> SendAsync<TMessage, TResponse>(RoutedMessage<TMessage, TResponse> message, CancellationToken cancellationToken = default)
		where TMessage : class
	{
		var task = new TaskCompletionSource<TResponse>();

		var requestQueueName = $"{_options.QueueName}${message.Channel}$";

		using var channel = await _connection.CreateChannelAsync();

		await CheckQueueAsync(channel, requestQueueName);

		var responseQueueName = await channel.ResponseQueueDeclareAsync(cancellationToken);
		var consumer = new AsyncEventingBasicConsumer(channel);

		consumer.ReceivedAsync += OnReceivedAsync;

		var typeName = message.GetTypeName();

		var props = new BasicProperties();
		props.Headers ??= new Dictionary<string, object>();
		props.Headers[Constants.MessageHeaders.MessageType] = typeName;
		props.Type = typeName;
		props.CorrelationId = message.CorrelationId;
		props.ReplyTo = responseQueueName;

		await Policy.Handle<SocketException>()
		            .Or<BrokerUnreachableException>()
		            .WaitAndRetryAsync(_options.MaxFailureRetries, _ => TimeSpan.FromSeconds(1), (exception, _, retryCount, _) =>
		            {
			            _logger.LogError(exception, "Retry:{RetryCount}, {Message}", retryCount, exception.Message);
		            })
		            .ExecuteAsync(async () =>
		            {
			            var messageBody = await SerializeAsync(message, cancellationToken);
			            await channel.BasicPublishAsync("", requestQueueName, true, props, messageBody, cancellationToken);
			            await channel.BasicConsumeAsync(responseQueueName, true, consumer, cancellationToken: cancellationToken);

			            Delivered?.Invoke(this, new MessageDeliveredEventArgs(message.Data, null));
		            });

		var result = await task.Task;
		consumer.ReceivedAsync -= OnReceivedAsync;
		return result;

		async Task OnReceivedAsync(object sender, BasicDeliverEventArgs args)
		{
			if (args.BasicProperties.CorrelationId != message.CorrelationId)
			{
				return;
			}

			var body = args.Body.ToArray();
			var response = JsonConvert.DeserializeObject<RabbitMqReply<TResponse>>(Encoding.UTF8.GetString(body), Constants.SerializerSettings);
			task.SetResult(response.Result);
			await Task.CompletedTask;
		}
	}

	private static async Task CheckQueueAsync(IChannel channel, string requestQueueName)
	{
		var queueDeclare = await channel.DeclareQueuePassivelyAsync(requestQueueName);

		if (queueDeclare == null)
		{
			throw new MessageDeliverException("Channel not found in vhost '/'.");
		}

		if (queueDeclare.ConsumerCount < 1)
		{
			throw new MessageDeliverException("No consumer found for the channel.");
		}
	}

	/// <summary>
	/// Serializes the message to bytes.
	/// </summary>
	/// <typeparam name="TMessage"></typeparam>
	/// <param name="message"></param>
	/// <param name="cancellationToken"></param>
	/// <returns></returns>
	private static async Task<byte[]> SerializeAsync<TMessage>(RoutedMessage<TMessage> message, CancellationToken cancellationToken = default)
		where TMessage : class
	{
		if (message == null)
		{
			return [];
		}

		await using var stream = new MemoryStream();
		// The default UTF8Encoding emits the BOM will cause the RabbitMQ client to fail to deserialize the message.
		using (var writer = new StreamWriter(stream, new UTF8Encoding(false)))
		{
			using var jsonWriter = new JsonTextWriter(writer);

			JsonSerializer.CreateDefault().Serialize(jsonWriter, message);

			await jsonWriter.FlushAsync(cancellationToken);
#if NET8_0_OR_GREATER
			await writer.FlushAsync(cancellationToken);
#else
			await writer.FlushAsync();
#endif
		}

		return stream.ToArray();
	}
}