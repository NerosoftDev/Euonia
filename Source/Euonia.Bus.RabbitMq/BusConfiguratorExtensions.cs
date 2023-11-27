using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace Nerosoft.Euonia.Bus.RabbitMq;

/// <summary>
/// Service bus extensions for <see cref="IBusConfigurator"/>.
/// </summary>
public static class BusConfiguratorExtensions
{
	/// <summary>
	/// Adds the RabbitMQ message transporter.
	/// </summary>
	/// <param name="configurator"></param>
	/// <param name="configuration"></param>
	public static void UseRabbitMq(this IBusConfigurator configurator, Action<RabbitMqMessageBusOptions> configuration)
	{
		configurator.Service.Configure(configuration);
		configurator.Service.TryAddSingleton<RabbitMqDispatcher>();
		configurator.Service.AddTransient<RabbitMqQueueConsumer>();
		configurator.Service.AddTransient<RabbitMqTopicSubscriber>();
		configurator.SerFactory<RabbitMqBusFactory>();
	}
}