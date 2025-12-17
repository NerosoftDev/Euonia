namespace Nerosoft.Euonia.Bus;

internal class OverridableMessageConvention : IMessageConvention
{
	private readonly IMessageConvention _innerConvention;
	private Func<Type, bool> _isUnicastType, _isMulticastType, _isRequestType;

	public OverridableMessageConvention(IMessageConvention innerConvention)
	{
		_innerConvention = innerConvention;
	}

	public string Name => $"Override with {_innerConvention.Name}";

	bool IMessageConvention.IsUnicastType(Type messageType)
	{
		return IsUnicastType(messageType);
	}

	bool IMessageConvention.IsMulticastType(Type messageType)
	{
		return IsMulticastType(messageType);
	}

	bool IMessageConvention.IsRequestType(Type messageType)
	{
		return IsRequestType(messageType);
	}

	public Func<Type, bool> IsUnicastType
	{
		get => _isUnicastType ?? _innerConvention.IsUnicastType;
		set => _isUnicastType = value;
	}

	public Func<Type, bool> IsMulticastType
	{
		get => _isMulticastType ?? _innerConvention.IsMulticastType;
		set => _isMulticastType = value;
	}

	public Func<Type, bool> IsRequestType
	{
		get => _isRequestType ?? _innerConvention.IsRequestType;
		set => _isRequestType = value;
	}

	public void DefineUnicastType(Func<Type, bool> convention)
	{
		_isUnicastType = convention;
	}

	public void DefineMulticastType(Func<Type, bool> convention)
	{
		_isMulticastType = convention;
	}
	
	public void DefineRequestType(Func<Type, bool> convention)
	{
		_isRequestType = convention;
	}
}