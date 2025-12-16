using System.Reflection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Nerosoft.Euonia.Bus;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extension methods for service collection to add message bus.
/// </summary>
public static class ServiceCollectionExtensions
{
	/// <summary>
	/// Adds the message bus to the service collection.
	/// </summary>
	/// <param name="config"></param>
	/// <param name="services">The <see cref="IServiceCollection"/> inatance.</param>
	/// <returns></returns>
	public static IBusConfigurator AddServiceBus(this IServiceCollection services, Action<BusConfigurator> config = null)
	{
		var configurator = Singleton<BusConfigurator>.Get(() => new BusConfigurator(services));

		config?.Invoke(configurator);

		var handlerTypes = configurator.Registrations
		                               .Select(t => t.HandlerType)
		                               .Distinct()
		                               .ToList();

		foreach (var handlerType in handlerTypes)
		{
			services.TryAddTransient(handlerType);
		}

		services.AddSingleton<IBusConfigurator>(_ => configurator);

		services.TryAddSingleton<IHandlerContext>(provider =>
		{
			var context = new HandlerContext(provider);

			var registerMethod = typeof(HandlerContext).GetMethod(nameof(HandlerContext.Register), 2, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly, []);

			foreach (var registration in configurator.Registrations)
			{
				if (registration.HandlerType.IsAssignableTo(typeof(IHandler<>).MakeGenericType(registration.MessageType)))
				{
					registerMethod?.MakeGenericMethod(registration.MessageType, registration.HandlerType).Invoke(context, null);
				}
				else
				{
					context.Register(registration);
				}
			}

			return context;
		});

		services.TryAddSingleton<IMessageConvention>(_ => configurator.ConventionBuilder.Convention);
		foreach (var (name, builder) in configurator.StrategyBuilders)
		{
			services.TryAddKeyedSingleton<ITransportStrategy>(name, (_, _) => builder.Strategy);
		}

		services.TryAddTransient<IBus, ServiceBus>();
		services.TryAddSingleton<IDispatcher, StrategicDispatcher>();
		services.AddHostedService<RecipientActivator>();

		return configurator;
	}
}