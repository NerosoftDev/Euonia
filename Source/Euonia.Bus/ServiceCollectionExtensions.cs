using Microsoft.Extensions.DependencyInjection.Extensions;
using Nerosoft.Euonia.Bus;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extension methods for service collection to add message bus.
/// </summary>
public static class ServiceCollectionExtensions
{
	/// <summary>
	/// Register message bus.
	/// </summary>
	/// <param name="services"></param>
	/// <param name="config"></param>
	public static void AddServiceBus(this IServiceCollection services, Action<BusConfigurator> config)
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
}