using System.Collections.Concurrent;

namespace Nerosoft.Euonia.Bus;

/// <summary>
/// 
/// </summary>
public class MessageConvention : IMessageConvention
{
	private readonly OverridableMessageConvention _defaultConvention = new(new DefaultMessageConvention());
	private readonly List<IMessageConvention> _conventions = new();
	private readonly ConventionCache _topicConventionCache = new();
	private readonly ConventionCache _queueConventionCache = new();
	private readonly ConventionCache _requestConventionCache = new();

	/// <summary>
	/// Determines whether the specified type is a command.
	/// </summary>
	/// <param name="messageType"></param>
	/// <returns></returns>
	/// <exception cref="ArgumentNullException"></exception>
	public bool IsQueueType(Type messageType)
	{
		ArgumentNullException.ThrowIfNull(messageType);

		return _topicConventionCache.Apply(messageType, handle =>
		{
			var t = Type.GetTypeFromHandle(handle);
			return _conventions.Any(x => x.IsQueueType(t));
		});
	}

	/// <summary>
	/// Determines whether the specified type is an event.
	/// </summary>
	/// <param name="messageType"></param>
	/// <returns></returns>
	/// <exception cref="ArgumentNullException"></exception>
	public bool IsTopicType(Type messageType)
	{
		ArgumentNullException.ThrowIfNull(messageType);

		return _queueConventionCache.Apply(messageType, handle =>
		{
			var t = Type.GetTypeFromHandle(handle);
			return _conventions.Any(x => x.IsTopicType(t));
		});
	}

	/// <inheritdoc />
	public bool IsRequestType(Type messageType)
	{
		ArgumentNullException.ThrowIfNull(messageType);

		return _requestConventionCache.Apply(messageType, handle =>
		{
			var t = Type.GetTypeFromHandle(handle);
			return _conventions.Any(x => x.IsRequestType(t));
		});
	}

	internal void DefineQueueTypeConvention(Func<Type, bool> convention)
	{
		_defaultConvention.DefineQueueType(convention);
	}

	internal void DefineTopicTypeConvention(Func<Type, bool> convention)
	{
		_defaultConvention.DefineTopicType(convention);
	}

	internal void DefineRequestTypeConvention(Func<Type, bool> convention)
	{
		_defaultConvention.DefineRequestType(convention);
	}

	internal void DefineTypeConvention(Func<Type, MessageConventionType> convention)
	{
	}

	internal void Add(params IMessageConvention[] conventions)
	{
		if (conventions == null || conventions.Length == 0)
		{
			throw new ArgumentException(@"At least one convention must be provided.", nameof(conventions));
		}

		_conventions.AddRange(conventions);
	}

	/// <summary>
	/// Gets the registered conventions.
	/// </summary>
	internal string[] RegisteredConventions => _conventions.Select(x => x.Name).ToArray();

	/// <inheritdoc/>
	public string Name => "Default";

	private class ConventionCache
	{
		public bool Apply(Type type, Func<RuntimeTypeHandle, bool> convention)
		{
			return _cache.GetOrAdd(type.TypeHandle, convention);
		}

		// ReSharper disable once UnusedMember.Local

		/// <summary>
		/// Reset cache.
		/// </summary>
		public void Reset()
		{
			_cache.Clear();
		}

		private readonly ConcurrentDictionary<RuntimeTypeHandle, bool> _cache = new();
	}
}