namespace Nerosoft.Euonia.Modularity;

/// <summary>
/// Defines the interface of a module manager.
/// </summary>
public interface IModuleManager
{
    /// <summary>
    /// Initializes the modules.
    /// </summary>
    /// <param name="context"></param>
    void InitializeModules(ApplicationInitializationContext context);

    /// <summary>
    /// Unload the modules.
    /// </summary>
    /// <param name="context"></param>
    void UnloadModules(ApplicationShutdownContext context);
}
