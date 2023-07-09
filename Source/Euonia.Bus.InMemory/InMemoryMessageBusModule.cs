using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Nerosoft.Euonia.Modularity;

namespace Nerosoft.Euonia.Bus.InMemory;

/// <summary>
/// Import this module to use the in-memory message bus.
/// </summary>
[DependsOn(typeof(MessageBusModule))]
public class InMemoryMessageBusModule : ModuleContextBase
{
    /// <inheritdoc />
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.TryAddSingleton<MessageConversionDelegate>(_ => MessageConverter.Convert);
        context.Services.AddInMemoryCommandBus();
        context.Services.AddInMemoryEventBus();
    }
}