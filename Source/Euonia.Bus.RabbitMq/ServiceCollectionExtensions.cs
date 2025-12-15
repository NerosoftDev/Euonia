using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Nerosoft.Euonia.Bus;
using Nerosoft.Euonia.Bus.RabbitMq;
using RabbitMQ.Client;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extension methods for IServiceCollection to add RabbitMQ Bus services.
/// </summary>
public static class ServiceCollectionExtensions
{
	/// <summary>
	/// Extension methods for IServiceCollection to add RabbitMQ Bus services.
	/// </summary>
	/// <param name="services"></param>
	extension(IServiceCollection services)
	{
		/// <summary>
		/// Adds the RabbitMQ bus services to the service collection.
		/// </summary>
		/// <param name="configureOptions"></param>
		/// <returns></returns>
		/// <exception cref="InvalidOperationException"></exception>
		public IServiceCollection AddRabbitMqBus(Action<RabbitMqBusOptions> configureOptions = null)
		{
			if (configureOptions != null)
			{
				services.Configure(configureOptions);
			}

			// Registers a singleton IConnectionFactory implementation using the configured RabbitMQ options.
			services.TryAddSingleton<IConnectionFactory>(provider =>
			{
				var options = provider.GetService<IOptions<RabbitMqBusOptions>>()?.Value;

				if (options == null)
				{
					throw new InvalidOperationException("RabbitMqMessageBusOptions was not configured.");
				}

				// Creates and returns a RabbitMQ connection factory using the provided connection URI.
				var factory = new ConnectionFactory { Uri = new Uri(options.Connection) };
				return factory;
			});

			// Registers a singleton implementation of IPersistentConnection.
			services.TryAddSingleton<IPersistentConnection, DefaultPersistentConnection>();

			// Registers RabbitMQ transport-related services.
			services.TryAddSingleton<RabbitMqTransport>();
			services.TryAddTransient<IRecipientRegistrar, RabbitMqRecipientRegistrar>();
			services.TryAddTransient<RabbitMqQueueConsumer>();
			services.TryAddTransient<RabbitMqTopicSubscriber>();

			return services;
		}

		/// <summary>
		/// Adds a RabbitMQ bus handle strategy to the service collection.
		/// </summary>
		/// <param name="configure"></param>
		/// <returns></returns>
		public IServiceCollection AddRabbitMqBusStrategy(Action<TransportStrategyBuilder> configure)
		{
			var builder = new TransportStrategyBuilder();
			configure(builder);
			services.AddKeyedSingleton<ITransportStrategy>(typeof(RabbitMqTransport), (_, _) => builder.Strategy);
			return services;
		}
	}
}