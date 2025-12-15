namespace Nerosoft.Euonia.Bus;

internal class OverridableTransportStrategy : ITransportStrategy
{
	private readonly ITransportStrategy _innerStrategy;
	private Func<Type, bool> _outboundEvaluator, _inboundEvaluator;

	public OverridableTransportStrategy(ITransportStrategy innerStrategy)
	{
		_innerStrategy = innerStrategy;
	}

	public string Name => $"Override with {_innerStrategy.Name}";

	bool ITransportStrategy.Outbound(Type messageType)
	{
		return Outbound(messageType);
	}

	bool ITransportStrategy.Inbound(Type messageType)
	{
		return Inbound(messageType);
	}

	public Func<Type, bool> Outbound
	{
		get => _outboundEvaluator ?? _innerStrategy.Outbound;
		set => _outboundEvaluator = value;
	}

	public Func<Type, bool> Inbound
	{
		get => _inboundEvaluator ?? _innerStrategy.Inbound;
		set => _inboundEvaluator = value;
	}

	public void DefineOutboundStrategy(Func<Type, bool> strategy)
	{
		_outboundEvaluator = strategy;
	}

	public void DefineInboundStrategy(Func<Type, bool> strategy)
	{
		_inboundEvaluator = strategy;
	}
}