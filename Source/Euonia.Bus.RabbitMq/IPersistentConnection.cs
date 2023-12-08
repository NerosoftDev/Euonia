using RabbitMQ.Client;

namespace Nerosoft.Euonia.Bus.RabbitMq;

/// <summary>
/// Defines the interface of a RabbitMQ persistent connection.
/// </summary>
public interface IPersistentConnection : IDisposable
{
	/// <summary>
	/// Gets a value indicating whether the connection is connected.
	/// </summary>
	bool IsConnected { get; }

	/// <summary>
	/// Tries to connect to RabbitMQ.
	/// </summary>
	/// <returns></returns>
	bool TryConnect();

	/// <summary>
	/// Creates a channel.
	/// </summary>
	/// <returns></returns>
	IModel CreateChannel();
}