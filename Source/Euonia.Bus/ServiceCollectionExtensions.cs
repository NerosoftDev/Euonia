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

				var registerMethod = typeof(HandlerContext).GetMethod(nameof(HandlerContext.Register), 2, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly,[]);

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
			services.TryAddSingleton<IBus, ServiceBus>();
			services.AddHostedService<RecipientActivator>();

			return configurator;
		}
	}
}