namespace Nerosoft.Euonia.Bus;

/// <summary>
/// 
/// </summary>
public interface IMessageEnvelope
{
	/// <summary>
	/// Gets or sets the identifier of the message.
	/// </summary>
	/// <value>
	/// The identifier of the message.
	/// </value>
	string MessageId { get; }

	/// <summary>
	/// Gets the correlation identifier.
	/// </summary>
	string CorrelationId { get; }

	/// <summary>
	/// Gets the conversation identifier.
	/// </summary>
	string ConversationId { get; }

	/// <summary>
	/// Gets the request trace identifier.
	/// </summary>
	string RequestTraceId { get; }

	/// <summary>
	/// Gets or sets the message channel.
	/// </summary>
	string Channel { get; set; }
}