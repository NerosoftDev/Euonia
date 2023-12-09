namespace Nerosoft.Euonia.Domain;

/// <summary>
/// 
/// </summary>
public interface IMessage
{
	/// <summary>
	/// Gets the message identifier.
	/// </summary>
	string MessageId { get; }
}