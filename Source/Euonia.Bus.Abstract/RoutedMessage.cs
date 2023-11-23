using System.Runtime.Serialization;

namespace Nerosoft.Euonia.Bus;

/// <summary>
/// 
/// </summary>
[Serializable]
public class RoutedMessage<TData> : IRoutedMessage
	where TData : class
{
	/// <summary>
	/// Initializes a new instance of the <see cref="RoutedMessage{TData}"/> class.
	/// </summary>
	public RoutedMessage()
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="RoutedMessage{TData}"/> class.
	/// </summary>
	/// <param name="data">The data.</param>
	/// <param name="channel"></param>
	public RoutedMessage(TData data, string channel)
	{
		Data = data;
		Channel = channel;
	}

	/// <summary>
	/// The message type key
	/// </summary>
	private const string MESSAGE_TYPE_KEY = "$nerosoft.euonia:message.type";

	/// <inheritdoc />
	[DataMember]
	public string MessageId { get; set; } = Guid.NewGuid().ToString();

	/// <inheritdoc />
	[DataMember]
	public string CorrelationId { get; }

	/// <inheritdoc />
	[DataMember]
	public string ConversationId { get; }

	/// <inheritdoc />
	[DataMember]
	public string RequestTraceId { get; }

	/// <inheritdoc />
	[DataMember]
	public string Channel { get; set; }

	/// <summary>
	/// Gets or sets the timestamp that describes when the message occurs.
	/// </summary>
	/// <value>
	/// The timestamp that describes when the message occurs.
	/// </value>
	[DataMember]
	public long Timestamp { get; set; }

	/// <summary>
	/// Gets a <see cref="MessageMetadata"/> instance that contains the metadata information of the message.
	/// </summary>
	[DataMember]
	public MessageMetadata Metadata { get; set; } = new();

	object IRoutedMessage.Data => Data;

	/// <inheritdoc cref="Data"/>
	private TData _data;

	/// <summary>
	/// Gets or sets the payload of the message.
	/// </summary>
	[DataMember]
	public TData Data
	{
		get => _data;
		set
		{
			_data = value;
			if (value != null)
			{
				Metadata[MESSAGE_TYPE_KEY] = value.GetType().AssemblyQualifiedName;
			}
		}
	}

	/// <summary>
	/// Gets the .NET CLR assembly qualified name of the message.
	/// </summary>
	/// <returns>
	/// The assembly qualified name of the message.
	/// </returns>
	public string GetTypeName() => Metadata[MESSAGE_TYPE_KEY] as string;

	/// <summary>
	/// Returns a <see cref="string"/> that represents this instance.
	/// </summary>
	/// <returns>
	/// A <see cref="string"/> that represents this instance.
	/// </returns>
	public override string ToString() => $"{MessageId}:{{GetTypeName()}}";
}

