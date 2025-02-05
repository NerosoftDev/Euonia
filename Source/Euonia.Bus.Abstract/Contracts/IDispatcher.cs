namespace Nerosoft.Euonia.Bus;

/// <summary>
/// 
/// </summary>
public interface IDispatcher
{
	/// <summary>
	/// Gets the transport for the specified message type.
	/// </summary>
	/// <param name="messageType">The type of message to be transport.</param>
	/// <returns></returns>
	ITransport CreateTransport(Type messageType);
	
	/// <summary>
	/// 
	/// </summary>
	/// <param name="identifier"></param>
	/// <returns></returns>
	ITransport CreateTransport(string identifier);
}