namespace Nerosoft.Euonia.Domain;

/// <summary>
/// Represents a command message.
/// </summary>
public interface ICommand : IMessage
{
	/// <summary>
	/// Gets the command identifier.
	/// </summary>
	string CommandId { get; }

	/// <summary>
	/// Override the message identifier.
	/// </summary>
	string IMessage.MessageId => CommandId;
}