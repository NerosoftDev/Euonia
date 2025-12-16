using System.Collections.Concurrent;
using Microsoft.Extensions.DependencyInjection;

namespace Nerosoft.Euonia.Bus;

/// <summary>
/// The strategic dispatcher.
/// </summary>
internal class StrategicDispatcher : IDispatcher
{
	private readonly ConcurrentDictionary<Type, IReadOnlyList<Type>> _transportCache = new();

	private readonly IMessageConvention _messageConvention;
	private readonly IServiceProvider _serviceProvider;
	private readonly IBusConfigurator _configurator;

	/// <summary>
	/// Initializes a new instance of the <see cref="StrategicDispatcher"/> class.
	/// </summary>
	/// <param name="serviceProvider"></param>
	/// <param name="messageConvention"></param>
	/// <param name="configurator"></param>
	public StrategicDispatcher(IServiceProvider serviceProvider, IMessageConvention messageConvention, IBusConfigurator configurator)
	{
		_messageConvention = messageConvention;
		_serviceProvider = serviceProvider;
		_configurator = configurator;
	}

	/// <summary>
	/// Creates the transports for the specified message type.
	/// </summary>
	/// <param name="messageType"></param>
	/// <returns></returns>
	/// <exception cref="MessageTypeException"></exception>
	public IEnumerable<Type> Determine(Type messageType)
	{
		var transportTypes = _transportCache.GetOrAdd(messageType, _ =>
		{
			var list = new List<Type>();
			foreach (var type in _configurator.StrategyAssignedTypes)
			{
				var strategy = _serviceProvider.GetKeyedService<ITransportStrategy>(type);
				if (strategy.Outgoing(messageType))
				{
					list.Add(type);
				}
			}

			return list;
		});

		switch (transportTypes.Count)
		{
			case 0:
				throw new MessageTypeException("No transport is configured for the message type.");
			case > 1 when !_messageConvention.IsMulticastType(messageType):
				throw new MessageTypeException("Multiple transports are configured for a unicast message type.");
		}

		return transportTypes;
	}
}