namespace Nerosoft.Euonia.Bus;

/// <summary>
/// The default message convention.
/// </summary>
public class DefaultMessageConvention : IMessageConvention
{
	/// <inheritdoc />
	public string Name => "Default Message Convention";

	/// <inheritdoc />
	public bool IsCommandType(Type type)
	{
		if (type == null)
		{
			throw new ArgumentNullException(nameof(type), "Type cannot be null.");
		}

		return type.IsAssignableTo(typeof(ICommand)) && type != typeof(ICommand);
	}

	/// <inheritdoc />
	public bool IsEventType(Type type)
	{
		if (type == null)
		{
			throw new ArgumentNullException(nameof(type), "Type cannot be null.");
		}

		return type.IsAssignableTo(typeof(IEvent)) && type != typeof(IEvent);
	}
}