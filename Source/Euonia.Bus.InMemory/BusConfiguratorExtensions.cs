using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Nerosoft.Euonia.Bus.InMemory;

namespace Nerosoft.Euonia.Bus;

/// <summary>
/// Service bus extensions for <see cref="IBusConfigurator"/>.
/// </summary>
public static class BusConfiguratorExtensions
{
	/// <summary>
	/// Adds the in-memory message transporter.
	/// </summary>
	/// <param name="configurator"></param>
	/// <param name="configuration"></param>
	public static void UseInMemory(this IBusConfigurator configurator, Action<InMemoryBusOptions> configuration)
	{
		configurator.Service.Configure(configuration);
		configurator.Service.TryAddTransient<InMemoryQueueConsumer>();
		configurator.Service.TryAddTransient<InMemoryTopicSubscriber>();
		configurator.Service.AddKeyedSingleton<ITransport, InMemoryTransport>(InMemoryTransport.TransportIdentifier);
		configurator.Service.AddTransient<IRecipientRegistrar, InMemoryRecipientRegistrar>();
	}
}