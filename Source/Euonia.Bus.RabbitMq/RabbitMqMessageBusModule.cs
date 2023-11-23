using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Nerosoft.Euonia.Modularity;

namespace Nerosoft.Euonia.Bus.RabbitMq;

/// <summary>
/// 
/// </summary>
[DependsOn(typeof(MessageBusModule))]
public class RabbitMqMessageBusModule : ModuleContextBase
{
    /// <inheritdoc />
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.TryAddSingleton<MessageConvert>(_ => MessageConverter.Convert);
        context.Services.AddRabbitMqCommandBus();
        context.Services.AddRabbitMqEventBus();
    }
}