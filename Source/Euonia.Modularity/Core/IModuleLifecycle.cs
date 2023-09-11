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
    /// Releases the module.
    /// </summary>
    /// <param name="context">The application shutdown context.</param>
    /// <param name="module">The module context.</param>
    void Finalize(ApplicationShutdownContext context, IModuleContext module);
}
