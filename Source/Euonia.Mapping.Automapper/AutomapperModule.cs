using Microsoft.Extensions.DependencyInjection;
using Nerosoft.Euonia.Modularity;

namespace Nerosoft.Euonia.Mapping;

/// <summary>
/// The module used to configure automapper services.
/// </summary>
public class AutomapperModule : ModuleContextBase
{
	/// <inheritdoc />
	public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddAutomapper();
        context.Services.AddSingleton<ITypeAdapterFactory, AutomapperTypeAdapterFactory>();
    }

	/// <inheritdoc />
	public override void OnApplicationInitialization(ApplicationInitializationContext context)
    {
        var factory = context.ServiceProvider.GetService<ITypeAdapterFactory>();
        if (factory != null)
        {
            TypeAdapterFactory.SetCurrent(factory);
        }
    }
}