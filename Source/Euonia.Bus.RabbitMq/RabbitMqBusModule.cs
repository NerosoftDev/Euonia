using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Nerosoft.Euonia.Modularity;
using RabbitMQ.Client;

namespace Nerosoft.Euonia.Bus.RabbitMq;

/// <summary>
/// Represents the RabbitMQ Bus Module, responsible for configuring services related to RabbitMQ messaging.
/// </summary>
public class RabbitMqBusModule : ModuleContextBase
{
	/// <summary>
	/// Configures the services required for RabbitMQ messaging.
	/// </summary>
	/// <param name="context">The service configuration context.</param>
	public override void ConfigureServices(ServiceConfigurationContext context)
	{
		// Configures RabbitMQ message bus options from the application configuration.
		context.Services.Configure<RabbitMqBusOptions>(Configuration.GetSection("ServiceBus:RabbitMQ"));

		context.Services.AddRabbitMqBus();
		// context.Services.TryAddSingleton<IBusFactory, RabbitMqBusFactory>();
	}
}