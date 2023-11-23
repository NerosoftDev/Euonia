namespace Nerosoft.Euonia.Bus;

internal class OverridableMessageConvention : IMessageConvention
{
	private readonly IMessageConvention _innerConvention;
	private Func<Type, bool> _isCommandType, _isEventType;

	public OverridableMessageConvention(IMessageConvention innerConvention)
	{
		_innerConvention = innerConvention;
	}

	public string Name => $"Override with {_innerConvention.Name}";

	bool IMessageConvention.IsCommandType(Type type)
	{
		return IsCommandType(type);
	}

	bool IMessageConvention.IsEventType(Type type)
	{
		return IsEventType(type);
	}

	public Func<Type, bool> IsCommandType
	{
		get => _isCommandType ?? _innerConvention.IsCommandType;
		set => _isCommandType = value;
	}

	public Func<Type, bool> IsEventType
	{
		get => _isEventType ?? _innerConvention.IsEventType;
		set => _isEventType = value;
	}

	public void DefineCommandType(Func<Type, bool> convention)
	{
		_isCommandType = convention;
	}

	public void DefineEventType(Func<Type, bool> convention)
	{
		_isEventType = convention;
	}
}