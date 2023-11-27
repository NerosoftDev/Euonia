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

	bool IMessageConvention.IsQueueType(Type type)
	{
		return IsCommandType(type);
	}

	bool IMessageConvention.IsTopicType(Type type)
	{
		return IsEventType(type);
	}

	public Func<Type, bool> IsCommandType
	{
		get => _isCommandType ?? _innerConvention.IsQueueType;
		set => _isCommandType = value;
	}

	public Func<Type, bool> IsEventType
	{
		get => _isEventType ?? _innerConvention.IsTopicType;
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