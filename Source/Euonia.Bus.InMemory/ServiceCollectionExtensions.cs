using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Nerosoft.Euonia.Bus;
using Nerosoft.Euonia.Bus.InMemory;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Message bus extensions for <see cref="IServiceCollection"/>.
/// </summary>
public static class ServiceCollectionExtensions
{
	/// <summary>
	/// Adds the in-memory command bus to the service collection.
	/// </summary>
	/// <param name="services"></param>
	/// <returns></returns>
	public static IServiceCollection AddInMemoryCommandBus(this IServiceCollection services)
	{
		return services.AddSingleton<ICommandBus>(provider =>
		{
			var bus = ActivatorUtilities.GetServiceOrCreateInstance<CommandBus>(provider);
			var options = provider.GetService<IOptions<MessageHandlerOptions>>()?.Value;
			if (options != null)
			{
				foreach (var subscription in options.Registration)
				{
					bus.Subscribe(subscription.MessageType, subscription.HandlerType);
				}
			}

			{
			}
			return bus;
		});
	}

	/// <summary>
	/// Adds the in-memory event bus to the service collection.
	/// </summary>
	/// <param name="services"></param>
	/// <returns></returns>
	public static IServiceCollection AddInMemoryEventBus(this IServiceCollection services)
	{
		return services.AddSingleton<IEventBus>(provider =>
		{
			var bus = ActivatorUtilities.GetServiceOrCreateInstance<EventBus>(provider);
			var options = provider.GetService<IOptions<MessageHandlerOptions>>()?.Value;
			if (options != null)
			{
				foreach (var subscription in options.Registration)
				{
					if (subscription.MessageType != null)
					{
						bus.Subscribe(subscription.MessageType, subscription.HandlerType);
					}
					else
					{
						bus.Subscribe(subscription.MessageName, subscription.HandlerType);
					}
				}
			}

			{
			}

			return bus;
		});
	}

	/// <summary>
	/// Adds the in-memory message bus to the service collection.
	/// </summary>
	/// <param name="configurator"></param>
	/// <param name="configuration"></param>
	/// <exception cref="InvalidOperationException"></exception>
	/// <exception cref="ArgumentOutOfRangeException"></exception>
	public static void UseInMemory(this IBusConfigurator configurator, Action<InMemoryBusOptions> configuration)
	{
		configurator.Service.Configure(configuration);
		configurator.Service.TryAddSingleton(provider =>
		{
			var options = provider.GetService<IOptions<InMemoryBusOptions>>()?.Value;
			if (options == null)
			{
				throw new InvalidOperationException("The in-memory message dispatcher options is not configured.");
			}

			IMessenger messenger = options.MessengerReference switch
			{
				MessengerReferenceType.StrongReference => StrongReferenceMessenger.Default,
				MessengerReferenceType.WeakReference => WeakReferenceMessenger.Default,
				_ => throw new ArgumentOutOfRangeException(nameof(options.MessengerReference), options.MessengerReference, null)
			};
			foreach (var subscription in configurator.GetSubscriptions())
			{
				messenger.Register(new InMemorySubscriber(subscription), subscription);
			}

			return messenger;
		});
		configurator.Service.TryAddSingleton<InMemoryDispatcher>();
		configurator.SerFactory<InMemoryBusFactory>();
	}
}