﻿using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Polly;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Nerosoft.Euonia.Bus.RabbitMq;

/// <summary>
/// The <see cref="IDispatcher"/> implementation using RabbitMQ.
/// </summary>
public class RabbitMqDispatcher : IDispatcher
{
	/// <inheritdoc />
	public event EventHandler<MessageDispatchedEventArgs> Delivered;

	private readonly RabbitMqMessageBusOptions _options;
	private readonly IConnection _connection;
	private readonly ILogger<RabbitMqDispatcher> _logger;

	/// <summary>
	/// Initialize a new instance of <see cref="RabbitMqDispatcher"/>.
	/// </summary>
	/// <param name="connection"></param>
	/// <param name="options"></param>
	/// <param name="logger"></param>
	public RabbitMqDispatcher(IConnection connection, IOptions<RabbitMqMessageBusOptions> options, ILoggerFactory logger)
	{
		_logger = logger.CreateLogger<RabbitMqDispatcher>();
		_connection = connection;
		_options = options.Value;
	}

	/// <inheritdoc />
	public async Task PublishAsync<TMessage>(RoutedMessage<TMessage> message, CancellationToken cancellationToken = default)
		where TMessage : class
	{
		using (var channel = _connection.CreateModel())
		{
			var typeName = message.Data.GetType().GetFullNameWithAssemblyName();

			var props = channel.CreateBasicProperties();
			props.Headers ??= new Dictionary<string, object>();
			props.Headers[Constants.MessageHeaders.MessageType] = typeName;
			props.Type = typeName;

			await Policy.Handle<Exception>()
			            .WaitAndRetryAsync(_options.MaxFailureRetries, _ => TimeSpan.FromSeconds(3), (exception, _, retryCount, _) =>
			            {
				            _logger.LogError(exception, "Retry:{RetryCount}, {Message}", retryCount, exception.Message);
			            })
			            .ExecuteAsync(async () =>
			            {
				            var messageBody = await SerializeAsync(message, cancellationToken);

				            channel.ExchangeDeclare(_options.ExchangeName, _options.ExchangeType);
				            channel.BasicPublish(_options.ExchangeName, message.Channel, props, messageBody);

				            Delivered?.Invoke(this, new MessageDispatchedEventArgs(message.Data, null));
			            });
		}
	}

	/// <inheritdoc />
	public async Task SendAsync<TMessage>(RoutedMessage<TMessage> message, CancellationToken cancellationToken = default) where TMessage : class
	{
		using (var channel = _connection.CreateModel())
		{
			var typeName = message.Data.GetType().GetFullNameWithAssemblyName();

			var props = channel.CreateBasicProperties();
			props.Headers ??= new Dictionary<string, object>();
			props.Headers[Constants.MessageHeaders.MessageType] = typeName;
			props.Type = typeName;

			await Policy.Handle<Exception>()
			            .WaitAndRetryAsync(_options.MaxFailureRetries, _ => TimeSpan.FromSeconds(3), (exception, _, retryCount, _) =>
			            {
				            _logger.LogError(exception, "Retry:{RetryCount}, {Message}", retryCount, exception.Message);
			            })
			            .ExecuteAsync(async () =>
			            {
				            var messageBody = await SerializeAsync(message, cancellationToken);

				            channel.BasicPublish("", $"{_options.QueueName}${message.Channel}$", props, messageBody);

				            Delivered?.Invoke(this, new MessageDispatchedEventArgs(message.Data, null));
			            });
		}
	}

	/// <inheritdoc />
	public async Task<TResponse> SendAsync<TMessage, TResponse>(RoutedMessage<TMessage, TResponse> message, CancellationToken cancellationToken = default) where TMessage : class
	{
		var task = new TaskCompletionSource<TResponse>();

		using (var channel = _connection.CreateModel())
		{
			var replyQueueName = channel.QueueDeclare().QueueName;
			var consumer = new EventingBasicConsumer(channel);

			consumer.Received += (_, args) =>
			{
				if (args.BasicProperties.CorrelationId != message.CorrelationId)
				{
					return;
				}

				var body = args.Body.ToArray();
				var response = JsonConvert.DeserializeObject<TResponse>(Encoding.UTF8.GetString(body), Constants.SerializerSettings);

				task.SetResult(response);
			};

			var typeName = message.Data.GetType().GetFullNameWithAssemblyName();

			var props = channel.CreateBasicProperties();
			props.Headers ??= new Dictionary<string, object>();
			props.Headers[Constants.MessageHeaders.MessageType] = typeName;
			props.Type = typeName;
			props.CorrelationId = message.CorrelationId;
			props.ReplyTo = replyQueueName;

			await Policy.Handle<Exception>()
			            .WaitAndRetryAsync(_options.MaxFailureRetries, _ => TimeSpan.FromSeconds(1), (exception, _, retryCount, _) =>
			            {
				            _logger.LogError(exception, "Retry:{RetryCount}, {Message}", retryCount, exception.Message);
			            })
			            .ExecuteAsync(async () =>
			            {
				            var messageBody = await SerializeAsync(message, cancellationToken);
				            channel.BasicPublish("", $"{_options.QueueName}${message.Channel}$", props, messageBody);
				            channel.BasicConsume(consumer, replyQueueName, true);

				            Delivered?.Invoke(this, new MessageDispatchedEventArgs(message.Data, null));
			            });
		}

		return await task.Task;
	}

	private static async Task<byte[]> SerializeAsync<TMessage>(RoutedMessage<TMessage> message, CancellationToken cancellationToken = default)
		where TMessage : class
	{
		if (message == null)
		{
			return Array.Empty<byte>();
		}

		await using var stream = new MemoryStream();
		await using var writer = new StreamWriter(stream, Encoding.UTF8, 1024, true);
		using var jsonWriter = new JsonTextWriter(writer);

		JsonSerializer.Create(Constants.SerializerSettings).Serialize(jsonWriter, message);

		await jsonWriter.FlushAsync(cancellationToken);
		await writer.FlushAsync();

		return stream.ToArray();
	}
}