namespace Nerosoft.Euonia.Bus;

/// <summary>
/// The bus factory interface
/// </summary>
public interface IBusFactory
{
	/// <summary>
	/// Create a new message dispatcher
	/// </summary>
	/// <returns></returns>
	ITransport CreateDispatcher();
}