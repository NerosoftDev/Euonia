using Microsoft.Extensions.DependencyInjection.Extensions;
using Nerosoft.Euonia.Bus;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extension methods for service collection to add message bus.
/// </summary>
public static class ServiceCollectionExtensions
{
	/// <param name="services">The <see cref="IServiceCollection"/> inatance.</param>
	extension(IServiceCollection services)
	{
		/// <summary>
		/// Register message bus.
		/// </summary>
		/// <param name="config"></param>
		public IBusConfigurator AddServiceBus(Action<BusConfigurator> config)
		{
			var configurator = Singleton<BusConfigurator>.Get(() => new BusConfigurator(services));

			config?.Invoke(configurator);

			services.AddSingleton<IBusConfigurator>(_ => configurator);

			services.TryAddSingleton<IHandlerContext>(provider =>
			{
				var context = new HandlerContext(provider);
				foreach (var registration in configurator.Registrations)
				{
					context.Register(registration);
				}

				return context;
			});

			services.TryAddSingleton<IMessageConvention>(_ => configurator.ConventionBuilder.Convention);
			services.TryAddScoped<IBus, ServiceBus>();
			services.AddHostedService<RecipientActivator>();

			return configurator;
		}
	}
}