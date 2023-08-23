namespace Nerosoft.Euonia.Modularity;

public interface IModuleLifecycle : ITransientDependency
{
    void Initialize(ApplicationInitializationContext context, IModuleContext module);

    void Shutdown(ApplicationShutdownContext context, IModuleContext module);
}
