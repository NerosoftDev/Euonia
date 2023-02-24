using Microsoft.Extensions.DependencyInjection;

namespace Nerosoft.Euonia.Modularity;

public interface IModularityApplication : IModuleContainer, IDisposable
{
    Type StartupModuleType { get; }

    IServiceCollection Services { get; }

    IServiceProvider ServiceProvider { get; }

    void Shutdown();
}
