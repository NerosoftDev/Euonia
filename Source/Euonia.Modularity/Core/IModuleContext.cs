namespace Nerosoft.Euonia.Modularity;

public interface IModuleContext
{
    /// <summary>
    /// Represents the method would be called before <see cref="ConfigureServices"/> of all modules are executed.
    /// </summary>
    /// <param name="context"></param>
    void AheadConfigureServices(ServiceConfigurationContext context);

    /// <summary>
    /// Configure services of current module.
    /// </summary>
    /// <param name="context"></param>
    void ConfigureServices(ServiceConfigurationContext context);

    /// <summary>
    /// Represents the method would be called after <see cref="ConfigureServices"/> of all modules have been executed.
    /// </summary>
    /// <param name="context"></param>
    void AfterConfigureServices(ServiceConfigurationContext context);

    /// <summary>
    /// Execute configuration on application initializing.
    /// </summary>
    /// <param name="context"></param>
    void OnApplicationInitialization(ApplicationInitializationContext context);

    /// <summary>
    /// Represents the module logic would be executed on application shutdown.
    /// </summary>
    /// <param name="context"></param>
    void OnApplicationShutdown(ApplicationShutdownContext context);
}