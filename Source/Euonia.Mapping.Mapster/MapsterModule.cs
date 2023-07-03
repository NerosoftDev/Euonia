using Microsoft.Extensions.DependencyInjection;
using Nerosoft.Euonia.Modularity;

namespace Nerosoft.Euonia.Mapping;

public class MapsterModule : ModuleContextBase
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddMapster();
        context.Services.AddSingleton<ITypeAdapterFactory, MapsterTypeAdapterFactory>();
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