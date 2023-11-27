namespace Nerosoft.Euonia.Bus;

internal class OverridableMessageConvention : IMessageConvention
{
	private readonly IMessageConvention _innerConvention;
	private Func<Type, bool> _isQueueType, _isTopicType;

	public OverridableMessageConvention(IMessageConvention innerConvention)
	{
		_innerConvention = innerConvention;
	}

	public string Name => $"Override with {_innerConvention.Name}";

	bool IMessageConvention.IsQueueType(Type type)
	{
		return IsQueueType(type);
	}

	bool IMessageConvention.IsTopicType(Type type)
	{
		return IsTopicType(type);
	}

	public Func<Type, bool> IsQueueType
	{
		get => _isQueueType ?? _innerConvention.IsQueueType;
		set => _isQueueType = value;
	}

	public Func<Type, bool> IsTopicType
	{
		get => _isTopicType ?? _innerConvention.IsTopicType;
		set => _isTopicType = value;
	}

	public void DefineCommandType(Func<Type, bool> convention)
	{
		_isQueueType = convention;
	}

	public void DefineEventType(Func<Type, bool> convention)
	{
		_isTopicType = convention;
	}
}