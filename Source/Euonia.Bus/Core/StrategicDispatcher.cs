using System.Collections.Concurrent;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Nerosoft.Euonia.Bus;

/// <summary>
/// The strategic dispatcher.
/// </summary>
internal class StrategicDispatcher : IDispatcher
{
	private readonly ConcurrentDictionary<Type, IReadOnlyList<string>> _transportCache = new();

	private readonly IMessageConvention _messageConvention;
	private readonly IServiceProvider _serviceProvider;
	private readonly IBusConfigurator _configurator;
	private readonly string _defaultTransport;

	/// <summary>
	/// Initializes a new instance of the <see cref="StrategicDispatcher"/> class.
	/// </summary>
	/// <param name="serviceProvider"></param>
	/// <param name="messageConvention"></param>
	/// <param name="configurator"></param>
	/// <param name="configuration"></param>
	public StrategicDispatcher(IServiceProvider serviceProvider, IMessageConvention messageConvention, IBusConfigurator configurator, IConfiguration configuration)
	{
		_messageConvention = messageConvention;
		_serviceProvider = serviceProvider;
		_configurator = configurator;
		_defaultTransport = string.Collapse(configuration.GetValue<string>(Constants.DefaultTransportSection), _configurator.DefaultTransport);
	}

	/// <summary>
	/// Creates the transports for the specified message type.
	/// </summary>
	/// <param name="messageType"></param>
	/// <returns></returns>
	/// <exception cref="MessageTypeException"></exception>
	public IEnumerable<string> Determine(Type messageType)
	{
		var transportTypes = _transportCache.GetOrAdd(messageType, _ =>
		{
			var list = new List<string>();
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
				if (string.IsNullOrEmpty(_defaultTransport))
				{
					throw new MessageTypeException("No transport is configured for the message type.");
				}

				transportTypes = new List<string> { _defaultTransport };
				break;

			case > 1 when !_messageConvention.IsMulticastType(messageType):
				throw new MessageTypeException("Multiple transports are configured for a unicast message type.");
		}

		return transportTypes;
	}
}