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
			throw new ArgumentNullException(nameof(type), "Type cannot be null.");
		}

		return type.IsAssignableTo(typeof(IQueue)) && type != typeof(IQueue);
	}

	/// <inheritdoc />
	public bool IsTopicType(Type type)
	{
		if (type == null)
		{
			throw new ArgumentNullException(nameof(type), "Type cannot be null.");
		}

		return type.IsAssignableTo(typeof(ITopic)) && type != typeof(ITopic);
	}
}