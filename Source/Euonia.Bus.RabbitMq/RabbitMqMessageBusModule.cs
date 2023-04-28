using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Nerosoft.Euonia.Modularity;

namespace Nerosoft.Euonia.Bus.RabbitMq;

/// <summary>
/// 
/// </summary>
[DependsOn(typeof(MessageBusModule))]
[DependsOn(typeof(RabbitMqCommandBusModule), typeof(RabbitMqEventBusModule))]
public class RabbitMqMessageBusModule : ModuleContextBase
{
    /// <summary>
    /// 
    /// </summary>
    public const string CommandBusProviderName = "rabbitmq";

    /// <summary>
    /// 
    /// </summary>
    public const string EventBusProviderName = "rabbitmq";

    /// <inheritdoc />
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.TryAddSingleton<MessageConversionDelegate>(_ =>
        {
            return MessageConverter.Convert;
        });
        //context.Items.TryGetValue("COMMAND_BUS_PROVIDER", value =>
        //{
        //    if (value is CommandBusProviderName or null)
        //    {
        //        context.Services.AddRabbitCommandBus();
        //    }
        //});
        //context.Items.TryGetValue("EVENT_BUS_PROVIDER", value =>
        //{
        //    if (value is EventBusProviderName or null)
        //    {
        //        context.Services.AddRabbitCommandBus();
        //    }
        //});
    }
}

/// <summary>
/// 
/// </summary>
[DependsOn(typeof(MessageBusModule))]
public class RabbitMqCommandBusModule : ModuleContextBase
{
    /// <inheritdoc />
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddRabbitMqCommandBus();
    }
}

/// <summary>
/// 
/// </summary>
[DependsOn(typeof(MessageBusModule))]
public class RabbitMqEventBusModule : ModuleContextBase
{
    /// <inheritdoc />
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddRabbitMqEventBus();
    }
}