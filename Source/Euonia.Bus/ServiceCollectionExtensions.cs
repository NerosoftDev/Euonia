using System.Reflection;
using MediatR;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Nerosoft.Euonia.Bus;
using Nerosoft.Euonia.Domain;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// 
/// </summary>
public static class ServiceCollectionExtensions
{
    private static readonly string[] _handlerTypes =
    {
        typeof(INotificationHandler<>).FullName,
        typeof(IRequestHandler<,>).FullName
    };

    /// <summary>
    /// Add message handler.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="callback"></param>
    public static void AddMessageHandler(this IServiceCollection services, Action<object, MessageSubscribedEventArgs> callback = null)
    {
        services.AddSingleton<IMessageHandlerContext>(provider =>
        {
            var @delegate = provider.GetService<MessageConversionDelegate>();
            var context = new MessageHandlerContext(provider, @delegate);
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
    /// <param name="assembly"></param>
    /// <param name="callback"></param>
    public static void AddMessageHandler(this IServiceCollection services, Assembly assembly, Action<object, MessageSubscribedEventArgs> callback = null)
    {
        services.AddMessageHandler(() =>
        {
            var handlerTypes = assembly.GetTypes().Where(t => t.GetInterface(nameof(IMessageHandler)) != null && t.IsClass && !t.IsAbstract).ToList();
            return handlerTypes;
        }, callback);
    }

    /// <summary>
    /// Add message handler.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="handlerTypesFactory"></param>
    /// <param name="callback"></param>
    public static void AddMessageHandler(this IServiceCollection services, Func<IEnumerable<Type>> handlerTypesFactory, Action<object, MessageSubscribedEventArgs> callback = null)
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
    public static void AddMessageHandler(this IServiceCollection services, IEnumerable<Type> handlerTypes, Action<object, MessageSubscribedEventArgs> callback = null)
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

            if (handlerType.GetInterface(nameof(IMessageHandler)) == null)
            {
                continue;
            }

            var inheritedTypes = handlerType.GetInterfaces().Where(t => t.IsGenericType);

            foreach (var inheritedType in inheritedTypes)
            {
                if (inheritedType.Name.Contains(nameof(IMessageHandler)))
                {
                    continue;
                }

                if (inheritedType.GenericTypeArguments.Length == 0)
                {
                    continue;
                }

                var messageType = inheritedType.GenericTypeArguments[0];
                if (messageType.IsSubclassOf(typeof(Command)))
                {
                    var interfaceType = typeof(ICommandHandler<>).MakeGenericType(messageType);
                    services.TryAddScoped(interfaceType, handlerType);
                    services.TryAddScoped(handlerType);
                }
                else if (messageType.IsSubclassOf(typeof(Event)))
                {
                    var interfaceType = typeof(IEventHandler<>).MakeGenericType(messageType);

                    services.AddScoped(interfaceType, handlerType);
                    services.AddScoped(handlerType);
                }
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="services"></param>
    /// <param name="assemblies"></param>
    public static void AddMediatorHandler(this IServiceCollection services, params Assembly[] assemblies)
    {
        var types = assemblies.SelectMany(t => t.GetTypes());
        services.AddMediatorHandler(types.ToArray());
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="services"></param>
    /// <param name="types"></param>
    public static void AddMediatorHandler(this IServiceCollection services, params Type[] types)
    {
        foreach (var type in types)
        {
            if (type.IsAbstract || !type.IsClass)
            {
                continue;
            }

            var interfaces = type.FindInterfaces(HandlerInterfaceFilter, null);
            if (interfaces.Length == 0)
            {
                continue;
            }

            foreach (var @interface in interfaces)
            {
                services.AddTransient(@interface, type);
            }
        }
    }

    private static bool HandlerInterfaceFilter(Type type, object criteria)
    {
        var typeName = $"{type.Namespace}.{type.Name}";
        return _handlerTypes.Contains(typeName);
    }
}