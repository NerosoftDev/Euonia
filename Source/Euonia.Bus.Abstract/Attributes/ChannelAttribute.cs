using System.Reflection;

namespace Nerosoft.Euonia.Bus;

/// <summary>
/// Represents the attributed event has a specified name.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class ChannelAttribute : Attribute
{
	/// <summary>
	/// Gets the event name.
	/// </summary>
	public string Name { get; }

	/// <summary>
	/// Initialize a new instance of <see cref="ChannelAttribute"/>.
	/// </summary>
	/// <param name="name"></param>
	/// <exception cref="ArgumentNullException"></exception>
	public ChannelAttribute(string name)
	{
		if (string.IsNullOrWhiteSpace(name))
		{
			throw new ArgumentNullException(nameof(name));
		}

		Name = name;
	}

	/// <summary>
	/// 
	/// </summary>
	/// <typeparam name="TMessage"></typeparam>
	/// <returns></returns>
	public static string GetName<TMessage>()
	{
		return GetName(typeof(TMessage));
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="messageType"></param>
	/// <returns></returns>
	/// <exception cref="ArgumentNullException"></exception>
	public static string GetName(Type messageType)
	{
		if (messageType == null)
		{
			throw new ArgumentNullException(nameof(messageType));
		}

		return messageType.GetCustomAttribute<ChannelAttribute>()?.Name ?? messageType.Name;
	}
}