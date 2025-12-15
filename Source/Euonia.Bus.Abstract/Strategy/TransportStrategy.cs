using System.Collections.Concurrent;

namespace Nerosoft.Euonia.Bus;

/// <summary>
/// The built-in transport strategy.
/// </summary>
public class TransportStrategy : ITransportStrategy
{
	private readonly OverridableTransportStrategy _defaultStrategy = new(new DefaultTransportStrategy());
	private readonly List<ITransportStrategy> _strategies = [];
	private readonly StrategyCache _outboundCache = new();
	private readonly StrategyCache _inboundCache = new();

	/// <summary>
	/// Initializes a new instance of the <see cref="TransportStrategy"/> class.
	/// </summary>
	public TransportStrategy()
	{
		_strategies.Add(_defaultStrategy);
	}

	/// <summary>
	/// The name of the strategy.
	/// </summary>
	public string Name => "Composite transport strategy";

	/// <summary>
	/// Determines whether the specified message type can be dispatched by the transport.
	/// </summary>
	/// <param name="messageType"></param>
	/// <returns></returns>
	public bool Outbound(Type messageType)
	{
		ArgumentNullException.ThrowIfNull(messageType);
		return _outboundCache.Apply(messageType, handle =>
		{
			var type = Type.GetTypeFromHandle(handle);
			return _strategies.Any(strategy => strategy.Outbound(type));
		});
	}

	/// <summary>
	/// Determines whether the specified message type can be received by the transport.
	/// </summary>
	/// <param name="messageType"></param>
	/// <returns></returns>
	public bool Inbound(Type messageType)
	{
		ArgumentNullException.ThrowIfNull(messageType);
		return _inboundCache.Apply(messageType, handle =>
		{
			var type = Type.GetTypeFromHandle(handle);
			return _strategies.Any(strategy => strategy.Inbound(type));
		});
	}

	internal void Add(params ITransportStrategy[] strategies)
	{
		if (strategies == null || strategies.Length == 0)
		{
			throw new ArgumentException(@"At least one strategy is required.", nameof(strategies));
		}

		_strategies.AddRange(strategies);
	}

	internal void DefineInboundStrategy(Func<Type, bool> strategy)
	{
		ArgumentNullException.ThrowIfNull(strategy);

		_defaultStrategy.DefineInboundStrategy(strategy);
	}

	internal void DefineOutboundStrategy(Func<Type, bool> strategy)
	{
		ArgumentNullException.ThrowIfNull(strategy);

		_defaultStrategy.DefineOutboundStrategy(strategy);
	}

	internal void ResetCache()
	{
		_outboundCache.Reset();
		_inboundCache.Reset();
	}

	private class StrategyCache
	{
		private readonly ConcurrentDictionary<RuntimeTypeHandle, bool> _cache = new();

		public bool Apply(Type messageType, Func<RuntimeTypeHandle, bool> strategy)
		{
			return _cache.GetOrAdd(messageType.TypeHandle, strategy);
		}

		public void Reset()
		{
			_cache.Clear();
		}
	}
}