namespace Nerosoft.Euonia.Bus.InMemory;

/// <summary>
/// The in-memory bus factory.
/// </summary>
public class InMemoryBusFactory : IBusFactory
{
	private readonly InMemoryDispatcher _dispatcher;

	/// <summary>
	/// Initializes a new instance of the <see cref="InMemoryBusFactory"/> class.
	/// </summary>
	/// <param name="dispatcher"></param>
	public InMemoryBusFactory(InMemoryDispatcher dispatcher)
	{
		_dispatcher = dispatcher;
	}

	/// <inheritdoc />
	public IDispatcher CreateDispatcher()
	{
		return _dispatcher;
	}
}