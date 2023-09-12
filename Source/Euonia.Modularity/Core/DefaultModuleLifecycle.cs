namespace Nerosoft.Euonia.Modularity;

/// <inheritdoc />
public class OnApplicationInitializationModuleLifecycle : ModuleLifecycleBase
{
    /// <inheritdoc />
    public override void Initialize(ApplicationInitializationContext context, IModuleContext module)
    {
        module.OnApplicationInitialization(context);
    }
}

/// <inheritdoc />
public class OnApplicationShutdownModuleLifecycle : ModuleLifecycleBase
{
    /// <inheritdoc />
    public override void Finalize(ApplicationShutdownContext context, IModuleContext module)
    {
        module.OnApplicationShutdown(context);
    }
}
