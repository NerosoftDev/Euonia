namespace Nerosoft.Euonia.Bus;

internal class OverridableMessageConvention : IMessageConvention
{
	private readonly IMessageConvention _innerConvention;
	private Func<Type, bool> _isCommandType, _isEventType, _isRequestType;

	public OverridableMessageConvention(IMessageConvention innerConvention)
	{
		_innerConvention = innerConvention;
	}

	public string Name => $"Override with {_innerConvention.Name}";

	bool IMessageConvention.IsCommandType(Type messageType)
	{
		return IsQueueType(messageType);
	}

	bool IMessageConvention.IsEventType(Type messageType)
	{
		return IsTopicType(messageType);
	}

	bool IMessageConvention.IsRequestType(Type messageType)
	{
		return IsRequestType(messageType);
	}

	public Func<Type, bool> IsQueueType
	{
		get => _isCommandType ?? _innerConvention.IsCommandType;
		set => _isCommandType = value;
	}

	public Func<Type, bool> IsTopicType
	{
		get => _isEventType ?? _innerConvention.IsEventType;
		set => _isEventType = value;
	}

	public Func<Type, bool> IsRequestType
	{
		get => _isRequestType ?? _innerConvention.IsRequestType;
		set => _isRequestType = value;
	}

	public void DefineQueueType(Func<Type, bool> convention)
	{
		_isCommandType = convention;
	}

	public void DefineTopicType(Func<Type, bool> convention)
	{
		_isEventType = convention;
	}
	
	public void DefineRequestType(Func<Type, bool> convention)
	{
		_isRequestType = convention;
	}
}