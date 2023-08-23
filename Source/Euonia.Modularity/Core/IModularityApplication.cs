using Microsoft.Extensions.DependencyInjection;

namespace Nerosoft.Euonia.Modularity;

/// <summary>
/// To be added.
/// </summary>
public interface IModularityApplication : IModuleContainer, IDisposable
{
    Type StartupModuleType { get; }

    IServiceCollection Services { get; }

    IServiceProvider ServiceProvider { get; }

    void Shutdown();
}
