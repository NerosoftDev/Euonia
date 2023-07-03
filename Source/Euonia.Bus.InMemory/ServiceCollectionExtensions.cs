﻿using Microsoft.Extensions.Options;
using Nerosoft.Euonia.Bus;
using Nerosoft.Euonia.Bus.InMemory;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInMemoryCommandBus(this IServiceCollection services)
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

    public static IServiceCollection AddInMemoryEventBus(this IServiceCollection services)
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