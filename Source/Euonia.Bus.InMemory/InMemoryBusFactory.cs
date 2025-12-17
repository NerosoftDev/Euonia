namespace Nerosoft.Euonia.Bus.InMemory;

/// <summary>
/// The in-memory bus factory.
/// </summary>
public class InMemoryBusFactory : IBusFactory
{
	private readonly InMemoryTransport _transport;

	/// <summary>
	/// Initializes a new instance of the <see cref="InMemoryBusFactory"/> class.
	/// </summary>
	/// <param name="transport"></param>
	public InMemoryBusFactory(InMemoryTransport transport)
	{
		_transport = transport;
	}

	/// <inheritdoc />
	public ITransport CreateDispatcher()
	{
		return _transport;
	}
}