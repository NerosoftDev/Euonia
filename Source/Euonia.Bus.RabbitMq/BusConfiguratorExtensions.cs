using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
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
		configurator.Service.Configure(configuration);

		configurator.Service.TryAddSingleton<IConnectionFactory>(provider =>
		{
			var options = provider.GetService<IOptions<RabbitMqMessageBusOptions>>()?.Value;

			if (options == null)
			{
				throw new InvalidOperationException("RabbitMqMessageBusOptions was not configured.");
			}

			var factory = new ConnectionFactory { Uri = new Uri(options.Connection) };
			return factory;
		});
		configurator.Service.TryAddSingleton<IPersistentConnection, DefaultPersistentConnection>();

		configurator.Service.TryAddTransient<RabbitMqQueueConsumer>();
		configurator.Service.TryAddTransient<RabbitMqTopicSubscriber>();

		configurator.Service.TryAddSingleton<RabbitMqDispatcher>();
		configurator.Service.AddTransient<IRecipientRegistrar, RabbitMqRecipientRegistrar>();
		configurator.SetFactory<RabbitMqBusFactory>();
	}
}