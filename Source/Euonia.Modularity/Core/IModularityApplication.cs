using Microsoft.Extensions.DependencyInjection;

namespace Nerosoft.Euonia.Modularity;

/// <summary>
/// To be added.
/// </summary>
public interface IModularityApplication : IModuleContainer, IDisposable
{
    /// <summary>
    /// Gets the startup module type.
    /// </summary>
    Type StartupModuleType { get; }

    /// <summary>
    /// Gets the <see cref="IServiceCollection"/> instance of current application.
    /// </summary>
    IServiceCollection Services { get; }

    /// <summary>
    /// Gets the <see cref="IServiceProvider"/> instance of current application.
    /// </summary>
    IServiceProvider ServiceProvider { get; }

    /// <summary>
    /// Shutdown the application.
    /// </summary>
    void Shutdown();
}
