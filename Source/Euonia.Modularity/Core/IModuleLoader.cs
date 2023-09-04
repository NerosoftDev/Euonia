using Microsoft.Extensions.DependencyInjection;

namespace Nerosoft.Euonia.Modularity;

/// <summary>
/// The contract of module loader.
/// </summary>
public interface IModuleLoader
{
    /// <summary>
    /// Loads the modules from application startup.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="startupModuleType"></param>
    /// <returns></returns>
    IModuleDescriptor[] LoadModules(IServiceCollection services, Type startupModuleType);
}
