using Microsoft.Extensions.DependencyInjection.Extensions;
using Nerosoft.Euonia.Bus;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Defines extension methods to register the service bus.
/// </summary>
public static class ServiceCollectionExtensions
{
	/// <summary>
	/// Register message bus.
	/// </summary>
	/// <param name="services"></param>
	/// <param name="config"></param>
	public static IBusConfigurator AddServiceBus(this IServiceCollection services, Action<BusConfigurator> config)
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
		services.TryAddSingleton<IDispatcher, DefaultDispatcher>();
		services.TryAddSingleton<IMessageConvention>(configurator.ConventionBuilder.Convention);
		services.TryAddScoped<IBus, ServiceBus>();
		services.AddHostedService<RecipientActivator>();
		return configurator;
	}
}