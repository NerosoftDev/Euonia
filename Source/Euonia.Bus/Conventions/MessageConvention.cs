using System.Collections.Concurrent;

namespace Nerosoft.Euonia.Bus;

/// <summary>
/// 
/// </summary>
public class MessageConvention
{
	private readonly OverridableMessageConvention _defaultConvention = new(new DefaultMessageConvention());
	private readonly List<IMessageConvention> _conventions = new();
	private readonly ConventionCache _commandConventionCache = new();
	private readonly ConventionCache _eventConventionCache = new();

	/// <summary>
	/// Determines whether the specified type is a command.
	/// </summary>
	/// <param name="type"></param>
	/// <returns></returns>
	/// <exception cref="ArgumentNullException"></exception>
	public bool IsQueueType(Type type)
	{
		ArgumentAssert.ThrowIfNull(type);

		return _commandConventionCache.Apply(type, handle =>
		{
			var t = Type.GetTypeFromHandle(handle);
			return _conventions.Any(x => x.IsQueueType(t));
		});
	}

	/// <summary>
	/// Determines whether the specified type is an event.
	/// </summary>
	/// <param name="type"></param>
	/// <returns></returns>
	/// <exception cref="ArgumentNullException"></exception>
	public bool IsTopicType(Type type)
	{
		ArgumentAssert.ThrowIfNull(type);

		return _eventConventionCache.Apply(type, handle =>
		{
			var t = Type.GetTypeFromHandle(handle);
			return _conventions.Any(x => x.IsTopicType(t));
		});
	}

	internal void DefineCommandTypeConvention(Func<Type, bool> convention)
	{
		_defaultConvention.DefineCommandType(convention);
	}

	internal void DefineEventTypeConvention(Func<Type, bool> convention)
	{
		_defaultConvention.DefineEventType(convention);
	}

	internal void Add(params IMessageConvention[] conventions)
	{
		if (conventions == null || conventions.Length == 0)
		{
			throw new ArgumentException(Resources.IDS_CONVENTION_PROVIDER_REQUIRED, nameof(conventions));
		}

		_conventions.AddRange(conventions);
	}

	/// <summary>
	/// Gets the registered conventions.
	/// </summary>
	internal string[] RegisteredConventions => _conventions.Select(x => x.Name).ToArray();

	private class ConventionCache
	{
		public bool Apply(Type type, Func<RuntimeTypeHandle, bool> convention)
		{
			return _cache.GetOrAdd(type.TypeHandle, convention);
		}

		public void Reset()
		{
			_cache.Clear();
		}

		private readonly ConcurrentDictionary<RuntimeTypeHandle, bool> _cache = new();
	}
}