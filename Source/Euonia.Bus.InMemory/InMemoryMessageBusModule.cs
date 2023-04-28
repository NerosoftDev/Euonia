using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Nerosoft.Euonia.Modularity;

namespace Nerosoft.Euonia.Bus.InMemory;

[DependsOn(typeof(MessageBusModule))]
[DependsOn(typeof(MemoryCommandBusModule), typeof(MemoryEventBusModule))]
public class InMemoryMessageBusModule : ModuleContextBase
{
    /// <summary>
    /// 
    /// </summary>
    public const string CommandBusProviderName = "inmemory";

    /// <summary>
    /// 
    /// </summary>
    public const string EventBusProviderName = "inmemory";

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
        //        context.Services.AddInMemoryCommandBus();
        //    }
        //});
        //context.Items.TryGetValue("EVENT_BUS_PROVIDER", value =>
        //{
        //    if (value is EventBusProviderName or null)
        //    {
        //        context.Services.AddInMemoryEventBus();
        //    }
        //});
    }
}

/// <summary>
/// 
/// </summary>
[DependsOn(typeof(MessageBusModule))]
public class MemoryCommandBusModule : ModuleContextBase
{
    /// <inheritdoc />
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddInMemoryCommandBus();
    }
}

/// <summary>
/// 
/// </summary>
[DependsOn(typeof(MessageBusModule))]
public class MemoryEventBusModule : ModuleContextBase
{
    /// <inheritdoc />
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddInMemoryEventBus();
    }
}