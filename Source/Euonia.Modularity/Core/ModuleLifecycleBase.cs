namespace Nerosoft.Euonia.Modularity;

public abstract class ModuleLifecycleBase : IModuleLifecycle
{
    public virtual void Initialize(ApplicationInitializationContext context, IModuleContext module)
    {
    }

    public virtual void Unload(ApplicationShutdownContext context, IModuleContext module)
    {
    }
}
