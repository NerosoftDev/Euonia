using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

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
		var options = new RabbitMqMessageBusOptions();
		configuration(options);

		configurator.Service.TryAddSingleton<IConnectionFactory>(provider =>
		{
			var factory = new ConnectionFactory { Uri = new Uri(options.Connection) };
			return factory;
		});
		configurator.Service.TryAddSingleton<IPersistentConnection, DefaultPersistentConnection>();

		configurator.Service.TryAddTransient<RabbitMqQueueConsumer>();
		configurator.Service.TryAddTransient<RabbitMqTopicSubscriber>();

		configurator.Service.AddKeyedSingleton<ITransport, RabbitMqTransport>("", (provider, _) =>
		{
			var logger = provider.GetService<ILoggerFactory>();
			return new RabbitMqTransport(null, options, logger);
		});
		
		configurator.Service.AddTransient<IRecipientRegistrar, RabbitMqRecipientRegistrar>();
	}
}