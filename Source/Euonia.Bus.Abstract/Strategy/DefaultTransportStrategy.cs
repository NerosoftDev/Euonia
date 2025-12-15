namespace Nerosoft.Euonia.Bus;

internal class DefaultTransportStrategy : ITransportStrategy
{
	public string Name { get; } = "Default Transport Strategy";

	public bool Outbound(Type messageType)
	{
		return false;
	}

	public bool Inbound(Type messageType)
	{
		return false;
	}
}