namespace Nerosoft.Euonia.Bus;

/// <summary>
/// The default message convention.
/// </summary>
public class DefaultMessageConvention : IMessageConvention
{
	/// <inheritdoc />
	public string Name => "Default Message Convention";

	/// <inheritdoc />
	public bool IsUnicastType(Type messageType)
	{
		ArgumentNullException.ThrowIfNull(messageType);
		
		return messageType.IsAssignableTo(typeof(IQueue)) && messageType != typeof(IQueue);
	}

	/// <inheritdoc />
	public bool IsMulticastType(Type messageType)
	{
		ArgumentNullException.ThrowIfNull(messageType);

		return messageType.IsAssignableTo(typeof(ITopic)) && messageType != typeof(ITopic);
	}

	/// <inheritdoc />
	public bool IsRequestType(Type messageType)
	{
		ArgumentNullException.ThrowIfNull(messageType);

		return messageType.IsAssignableToGeneric(typeof(IRequest<>));
	}
}