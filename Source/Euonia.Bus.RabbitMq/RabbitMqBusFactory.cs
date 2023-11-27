namespace Nerosoft.Euonia.Bus.RabbitMq;

/// <summary>
/// 
/// </summary>
public class RabbitMqBusFactory : IBusFactory
{
	private readonly RabbitMqDispatcher _dispatcher;

	/// <summary>
	/// Initializes a new instance of the <see cref="RabbitMqBusFactory"/> class.
	/// </summary>
	/// <param name="dispatcher"></param>
	public RabbitMqBusFactory(RabbitMqDispatcher dispatcher)
	{
		_dispatcher = dispatcher;
	}

	/// <inheritdoc />
	public IDispatcher CreateDispatcher()
	{
		return _dispatcher;
	}
}