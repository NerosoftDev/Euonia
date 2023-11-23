using Microsoft.Extensions.DependencyInjection.Extensions;
using Nerosoft.Euonia.Bus;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// 
/// </summary>
public static class ServiceCollectionExtensions
{
	/// <summary>
	/// Add message handler.
	/// </summary>
	/// <param name="services"></param>
	/// <param name="callback"></param>
	internal static void AddMessageHandler(this IServiceCollection services, Action<object, MessageSubscribedEventArgs> callback = null)
	{
		services.AddSingleton<IHandlerContext>(provider =>
		{
			var @delegate = provider.GetService<MessageConvert>();
			var context = new HandlerContext(provider, @delegate);
			context.MessageSubscribed += (sender, args) =>
			{
				callback?.Invoke(sender, args);
			};
			return context;
		});
	}

	/// <summary>
	/// Add message handler.
	/// </summary>
	/// <param name="services"></param>
	/// <param name="handlerTypesFactory"></param>
	/// <param name="callback"></param>
	internal static void AddMessageHandler(this IServiceCollection services, Func<IEnumerable<Type>> handlerTypesFactory, Action<object, MessageSubscribedEventArgs> callback = null)
	{
		var handlerTypes = handlerTypesFactory?.Invoke();

		services.AddMessageHandler(handlerTypes, callback);
	}

	/// <summary>
	/// Add message handler.
	/// </summary>
	/// <param name="services"></param>
	/// <param name="handlerTypes"></param>
	/// <param name="callback"></param>
	internal static void AddMessageHandler(this IServiceCollection services, IEnumerable<Type> handlerTypes, Action<object, MessageSubscribedEventArgs> callback = null)
	{
		services.AddMessageHandler(callback);

		if (handlerTypes == null)
		{
			return;
		}

		if (!handlerTypes.Any())
		{
			return;
		}

		foreach (var handlerType in handlerTypes)
		{
			if (!handlerType.IsClass)
			{
				continue;
			}

			if (handlerType.IsAbstract)
			{
				continue;
			}

			if (handlerType.GetInterface(nameof(IHandler)) == null)
			{
				continue;
			}

			var inheritedTypes = handlerType.GetInterfaces().Where(t => t.IsGenericType);

			foreach (var inheritedType in inheritedTypes)
			{
				if (inheritedType.Name.Contains(nameof(IHandler)))
				{
					continue;
				}

				if (inheritedType.GenericTypeArguments.Length == 0)
				{
					continue;
				}

				services.TryAddScoped(inheritedType, handlerType);
				services.TryAddScoped(handlerType);
			}
		}
	}

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
			var @delegate = provider.GetService<MessageConvert>();
			var context = new HandlerContext(provider, @delegate);
			foreach (var subscription in configurator.Registrations)
			{
				if (subscription.Type != null)
				{
					context.Register(subscription.Type, subscription.HandlerType, subscription.HandleMethod);
				}
				else
				{
					context.Register(subscription.Name, subscription.HandlerType, subscription.HandleMethod);
				}
			}

			return context;
		});
		services.AddSingleton<IBus, ServiceBus>();
	}
}