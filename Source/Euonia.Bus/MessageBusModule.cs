using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Nerosoft.Euonia.Modularity;

namespace Nerosoft.Euonia.Bus;

/// <summary>
/// 
/// </summary>
public class MessageBusModule : ModuleContextBase
{
    /// <inheritdoc />
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        {
            //context.Services.TryAddScoped<ServiceFactory>(provider => provider.GetService);
            context.Services.TryAddScoped<IMediator, Mediator>();

            var descriptor = new ServiceDescriptor(typeof(IMessageHandlerContext), provider =>
            {
                var @delegate = provider.GetService<MessageConversionDelegate>();
                var handlerContext = new MessageHandlerContext(provider, @delegate);

                // var options = provider.GetService<IOptions<MessageHandlerOptions>>()?.Value;
                // if (options != null)
                // {
                //     handlerContext.MessageSubscribed += (sender, args) =>
                //     {
                //         options.MessageSubscribed?.Invoke(sender, args);
                //     };
                //     foreach (var (message, handler) in options.Subscription)
                //     {
                //         handlerContext.Register(message, handler);
                //     }
                // }

                {
                }
                return handlerContext;
            }, ServiceLifetime.Singleton);
            context.Services.Replace(descriptor);
        }

        if (context.Services.All(t => t.ImplementationType != typeof(MessageBusActiveService)))
        {
            context.Services.AddHostedService<MessageBusActiveService>();
        }
    }
}