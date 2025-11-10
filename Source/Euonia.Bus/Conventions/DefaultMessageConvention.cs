namespace Nerosoft.Euonia.Bus;

/// <summary>
/// The default message convention.
/// </summary>
public class DefaultMessageConvention : IMessageConvention
{
	/// <inheritdoc />
	public string Name => "Default Message Convention";

	/// <inheritdoc />
	public bool IsQueueType(Type type)
	{
		if (type == null)
		{
			throw new ArgumentNullException(nameof(type), Resources.IDS_TYPE_CANNOT_NULL);
		}

		return type.IsAssignableTo(typeof(IQueue)) && type != typeof(IQueue);
	}

	/// <inheritdoc />
	public bool IsTopicType(Type type)
	{
		if (type == null)
		{
			throw new ArgumentNullException(nameof(type), Resources.IDS_TYPE_CANNOT_NULL);
		}

		return type.IsAssignableTo(typeof(ITopic)) && type != typeof(ITopic);
	}

	/// <inheritdoc />
	public bool IsRequestType(Type type)
	{
		if (type == null)
		{
			throw new ArgumentNullException(nameof(type), Resources.IDS_TYPE_CANNOT_NULL);
		}

		return type.IsAssignableToGeneric(typeof(IRequest<>));
	}
}