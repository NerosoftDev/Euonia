namespace Nerosoft.Euonia.Bus;

/// <summary>
/// 
/// </summary>
public interface IMessageContent
{
	/// <summary>
	/// Gets the message content length.
	/// </summary>
	long? Length { get; }

	/// <summary>
	/// Read the message body as a stream
	/// </summary>
	/// <returns></returns>
	Stream ReadAsStream();

	/// <summary>
	/// Read the message body as a byte array
	/// </summary>
	/// <returns></returns>
	byte[] ReadAsBytes();

	/// <summary>
	/// Read the message body as a string
	/// </summary>
	/// <returns></returns>
	string ReadAsString();
}