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
		public void AddServiceBus(Action<BusConfigurator> config)
		{
			var configurator = Singleton<BusConfigurator>.Get(() => new BusConfigurator(services));

			config?.Invoke(configurator);

			services.AddSingleton<IHandlerContext>(provider =>
			{
				var context = new HandlerContext(provider);
				foreach (var registration in configurator.Registrations)
				{
					context.Register(registration);
				}

				return context;
			});
			services.TryAddSingleton<IMessageConvention>(configurator.ConventionBuilder.Convention);
			services.TryAddScoped<IBus, ServiceBus>();
			services.AddHostedService<RecipientActivator>();
		}

		/// <summary>
		/// Register message bus.
		/// </summary>
		/// <param name="config"></param>
		public void AddServiceBus(Action<IServiceProvider, BusConfigurator> config)
		{
			services.AddSingleton(provider =>
			{
				var configurator = Singleton<BusConfigurator>.Get(() => new BusConfigurator(services));

				config?.Invoke(provider, configurator);
				return configurator;
			});
		
			services.AddSingleton<IBusConfigurator>(provider => provider.GetRequiredService<BusConfigurator>());

			services.TryAddSingleton<IHandlerContext>(provider =>
			{
				var context = new HandlerContext(provider);
				var configurator = provider.GetRequiredService<IBusConfigurator>()!;

				foreach (var registration in configurator.Registrations)
				{
					context.Register(registration);
				}

				return context;
			});

			services.TryAddSingleton<IMessageConvention>(provider =>
			{
				var configurator = provider.GetRequiredService<BusConfigurator>()!;
				return configurator.ConventionBuilder.Convention;
			});
		
			services.TryAddScoped<IBus, ServiceBus>();
			services.AddHostedService<RecipientActivator>();
		}
	}
}