namespace Nerosoft.Euonia.Bus;

/// <summary>
/// Defines a message pack basic information contract.
/// </summary>
public interface IRoutedMessage : IMessageEnvelope
{
	/// <summary>
	/// Gets the message creation timestamp.
	/// </summary>
	long Timestamp { get; }

	/// <summary>
	/// Gets a <see cref="MessageMetadata"/> instance that contains the metadata information of the message.
	/// </summary>
	MessageMetadata Metadata { get; }

	/// <summary>
	/// Gets the data of the message.
	/// </summary>
	object Data { get; }

	/// <summary>
	/// Gets the request authorization.
	/// </summary>
	string Authorization { get; set; }
}