namespace Nerosoft.Euonia.Modularity;

public interface IModuleContext
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="context"></param>
    void PreparatoryServices(ServiceConfigurationContext context);

    /// <summary>
    /// Configure services.
    /// </summary>
    /// <param name="context"></param>
    void ConfigureServices(ServiceConfigurationContext context);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="context"></param>
    void PostponeServices(ServiceConfigurationContext context);
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="context"></param>
    void OnApplicationInitialization(ApplicationInitializationContext context);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="context"></param>
    void OnApplicationShutdown(ApplicationShutdownContext context);
}