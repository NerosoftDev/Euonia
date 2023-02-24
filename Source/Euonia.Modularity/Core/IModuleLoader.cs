using Microsoft.Extensions.DependencyInjection;

namespace Nerosoft.Euonia.Modularity;

public interface IModuleLoader
{
    IModuleDescriptor[] LoadModules(IServiceCollection services, Type startupModuleType);
}
