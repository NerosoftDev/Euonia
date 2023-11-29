using System.Collections.Concurrent;

namespace Nerosoft.Euonia.Bus.RabbitMq;

/// <summary>
/// 
/// </summary>
public class MessageTypeCache
{
	private static readonly ConcurrentDictionary<string, Type> _messageTypes = new();

	/// <summary>
	/// 
	/// </summary>
	/// <param name="messageName"></param>
	/// <returns></returns>
	/// <exception cref="InvalidOperationException"></exception>
	public static Type GetMessageType(string messageName)
	{
		return _messageTypes.GetOrAdd(messageName, name =>
		{
			var type = Type.GetType(name);
			if (type == null)
			{
				throw new InvalidOperationException($"Could not find message type '{name}'.");
			}

			return type;
		});
	}
}