using System.Collections.Concurrent;
using System.Reflection;

namespace Nerosoft.Euonia.Bus;

internal class MessageChannelCache
{
	private static readonly Lazy<MessageChannelCache> _instance = new(() => new MessageChannelCache());

	private readonly ConcurrentDictionary<Type, string> _channels = new();

	public static MessageChannelCache Default => _instance.Value;

	public string GetOrAdd<TMessage>()
		where TMessage : class
	{
		return GetOrAdd(typeof(TMessage));
	}

	public string GetOrAdd(Type messageType)
	{
		return _channels.GetOrAdd(messageType, _ =>
		{
			var channelAttribute = messageType.GetCustomAttribute<ChannelAttribute>();
			return channelAttribute != null ? channelAttribute.Name : messageType.FullName;
		});
	}
}