namespace Nerosoft.Euonia.Modularity;

/// <summary>
/// Defines the interface of a module lifecycle.
/// </summary>
public interface IModuleLifecycle : ITransientDependency
{
    /// <summary>
    /// Initializes the module.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="module"></param>
    void Initialize(ApplicationInitializationContext context, IModuleContext module);

    /// <summary>
    /// Shutdowns the module.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="module"></param>
    void Unload(ApplicationShutdownContext context, IModuleContext module);
}
