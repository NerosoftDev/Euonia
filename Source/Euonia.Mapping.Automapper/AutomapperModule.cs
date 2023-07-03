using Microsoft.Extensions.DependencyInjection;
using Nerosoft.Euonia.Modularity;

namespace Nerosoft.Euonia.Mapping;

public class AutomapperModule : ModuleContextBase
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddAutomapper();
        context.Services.AddSingleton<ITypeAdapterFactory, AutomapperTypeAdapterFactory>();
    }

    public override void OnApplicationInitialization(ApplicationInitializationContext context)
    {
        var factory = context.ServiceProvider.GetService<ITypeAdapterFactory>();
        if (factory != null)
        {
            TypeAdapterFactory.SetCurrent(factory);
        }
    }
}