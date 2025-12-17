namespace Nerosoft.Euonia.Bus.RabbitMq;

/// <summary>
/// 
/// </summary>
public class RabbitMqBusFactory : IBusFactory
{
	private readonly RabbitMqTransport _transport;

	/// <summary>
	/// Initializes a new instance of the <see cref="RabbitMqBusFactory"/> class.
	/// </summary>
	/// <param name="transport"></param>
	public RabbitMqBusFactory(RabbitMqTransport transport)
	{
		_transport = transport;
	}

	/// <inheritdoc />
	public ITransport CreateDispatcher()
	{
		return _transport;
	}
}