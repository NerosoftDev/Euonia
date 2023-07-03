using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Nerosoft.Euonia.Bus;
using Nerosoft.Euonia.Bus.RabbitMq;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// 
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Add message bus.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <param name="optionKey"></param>
    /// <param name="action"></param>
    public static void AddRabbitMqMessageBus(this IServiceCollection services, IConfiguration configuration, string optionKey, Action<CommandBus> action = null)
    {
        services.Configure<RabbitMqMessageBusOptions>(configuration.GetSection(optionKey));
        if (action == null)
        {
            services.AddSingleton<ICommandBus, CommandBus>();
        }
        else
        {
            services.AddSingleton<ICommandBus>(provider =>
            {
                var commandBus = ActivatorUtilities.GetServiceOrCreateInstance<CommandBus>(provider);
                action(commandBus);
                return commandBus;
            });
        }

        services.AddSingleton<IEventBus, EventBus>();
    }

    /// <summary>
    /// Add message bus.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configureOptions"></param>
    /// <param name="action"></param>
    public static void AddRabbitMqMessageBus(this IServiceCollection services, Action<RabbitMqMessageBusOptions> configureOptions, Action<CommandBus> action = null)
    {
        services.Configure(configureOptions);
        if (action == null)
        {
            services.AddSingleton<ICommandBus, CommandBus>();
        }
        else
        {
            services.AddSingleton<ICommandBus>(provider =>
            {
                var commandBus = ActivatorUtilities.GetServiceOrCreateInstance<CommandBus>(provider);
                action(commandBus);
                return commandBus;
            });
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configureOptions"></param>
    /// <returns></returns>
    public static IServiceCollection AddRabbitMqConfiguration(this IServiceCollection services, Action<RabbitMqMessageBusOptions> configureOptions)
    {
        return services.Configure(configureOptions);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <param name="key"></param>
    /// <returns></returns>
    public static IServiceCollection AddRabbitMqConfiguration(this IServiceCollection services, IConfiguration configuration, string key)
    {
        return services.Configure<RabbitMqMessageBusOptions>(configuration.GetSection(key));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddRabbitMqCommandBus(this IServiceCollection services)
    {
        return services.AddSingleton<ICommandBus>(provider =>
        {
            var bus = ActivatorUtilities.GetServiceOrCreateInstance<CommandBus>(provider);
            var options = provider.GetService<IOptions<MessageHandlerOptions>>()?.Value;
            if (options != null)
            {
                foreach (var subscription in options.Subscription)
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
    /// 
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddRabbitMqEventBus(this IServiceCollection services)
    {
        return services.AddSingleton<IEventBus>(provider =>
        {
            var bus = ActivatorUtilities.GetServiceOrCreateInstance<EventBus>(provider);
            var options = provider.GetService<IOptions<MessageHandlerOptions>>()?.Value;
            if (options != null)
            {
                foreach (var subscription in options.Subscription)
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
}