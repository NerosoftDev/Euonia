using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Nerosoft.Euonia.Bus.RabbitMq;

/// <summary>
/// The base class for RabbitMQ message recipients.
/// </summary>
public abstract class RabbitMqQueueRecipient : DisposableObject
{
	/// <summary>
	/// Occurs when [message received].
	/// </summary>
	public event EventHandler<MessageReceivedEventArgs> MessageReceived;

	/// <summary>
	/// Occurs when [message acknowledged].
	/// </summary>
	public event EventHandler<MessageAcknowledgedEventArgs> MessageAcknowledged;

	/// <summary>
	/// Initializes a new instance of the <see cref="RabbitMqQueueRecipient"/> class.
	/// </summary>
	/// <param name="factory"></param>
	/// <param name="handler"></param>
	/// <param name="options"></param>
	protected RabbitMqQueueRecipient(ConnectionFactory factory, IHandlerContext handler, IOptions<RabbitMqMessageBusOptions> options)
	{
		Options = options.Value;
		Handler = handler;
		ConnectionFactory = factory;
	}

	/// <summary>
	/// 
	/// </summary>
	protected ConnectionFactory ConnectionFactory { get; }

	/// <summary>
	/// Gets the RabbitMQ message bus options.
	/// </summary>
	protected virtual RabbitMqMessageBusOptions Options { get; }

	/// <summary>
	/// Gets the message handler context instance.
	/// </summary>
	protected virtual IHandlerContext Handler { get; }

	// protected virtual void AcknowledgeMessage(ulong deliveryTag)
	// {
	// 	Channel.BasicAck(deliveryTag, false);
	// }

	internal abstract void Start(string channel);

	/// <summary>
	/// 
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="args"></param>
	protected abstract void HandleMessageReceived(object sender, BasicDeliverEventArgs args);

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
	/// <param name="args"></param>
	protected virtual void OnMessageReceived(MessageReceivedEventArgs args)
	{
		MessageReceived?.Invoke(this, args);
	}

	/// <summary>
	/// Serializes the message.
	/// </summary>
	/// <param name="message"></param>
	/// <returns></returns>
	protected virtual byte[] SerializeMessage(object message)
	{
		if (message == null)
		{
			return Array.Empty<byte>();
		}

		var json = JsonConvert.SerializeObject(message, Constants.SerializerSettings);
		return Encoding.UTF8.GetBytes(json);
	}

	/// <summary>
	/// Deserializes the message.
	/// </summary>
	/// <param name="message"></param>
	/// <param name="messageType"></param>
	/// <returns></returns>
	protected virtual IRoutedMessage DeserializeMessage(byte[] message, Type messageType)
	{
		var type = typeof(RoutedMessage<>).MakeGenericType(messageType);
		var json = Encoding.UTF8.GetString(message);
		return JsonConvert.DeserializeObject(json, type, Constants.SerializerSettings) as IRoutedMessage;
	}

	/// <summary>
	/// Gets the header value.
	/// </summary>
	/// <param name="header"></param>
	/// <param name="key"></param>
	/// <returns></returns>
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
}