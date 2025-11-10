namespace Nerosoft.Euonia.Bus;

internal class OverridableMessageConvention : IMessageConvention
{
	private readonly IMessageConvention _innerConvention;
	private Func<Type, bool> _isQueueType, _isTopicType, _isRequestType;

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

	bool IMessageConvention.IsRequestType(Type type)
	{
		return IsRequestType(type);
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

	public Func<Type, bool> IsRequestType
	{
		get => _isRequestType ?? _innerConvention.IsRequestType;
		set => _isRequestType = value;
	}

	public void DefineQueueType(Func<Type, bool> convention)
	{
		_isQueueType = convention;
	}

	public void DefineTopicType(Func<Type, bool> convention)
	{
		_isTopicType = convention;
	}
	
	public void DefineRequestType(Func<Type, bool> convention)
	{
		_isRequestType = convention;
	}
}