using Microsoft.Extensions.DependencyInjection;
using Nerosoft.Euonia.Dependency;

namespace Nerosoft.Euonia.Modularity;

/// <summary>
/// 
/// </summary>
public abstract class ModularityApplicationBase : IModularityApplication
{
    internal ModularityApplicationBase(Type startupModuleType, IServiceCollection services, Action<ApplicationCreationOptions> optionsAction)
    {
        StartupModuleType = startupModuleType;
        Services = services;

        services.TryAddObjectAccessor<IServiceProvider>();

        var options = new ApplicationCreationOptions(services);
        optionsAction?.Invoke(options);

        services.AddSingleton<IModularityApplication>(this);
        services.AddSingleton<IModuleContainer>(this);

        services.AddCoreServices();
        services.AddCoreServices(this, options);

        // ReSharper disable once VirtualMemberCallInConstructor
        Modules = LoadModules(services, options);
        // ReSharper disable once VirtualMemberCallInConstructor
        ConfigureServices();
    }

    /// <summary>
    /// 
    /// </summary>
    public Type StartupModuleType { get; }

    /// <summary>
    /// 
    /// </summary>
    public IServiceCollection Services { get; }

    /// <summary>
    /// 
    /// </summary>
    public IServiceProvider ServiceProvider { get; private set; }

    /// <summary>
    /// 
    /// </summary>
    public IReadOnlyList<IModuleDescriptor> Modules { get; }

    /// <summary>
    /// 
    /// </summary>
    public virtual void Dispose()
    {
    }

    /// <summary>
    /// 
    /// </summary>
    public void Shutdown()
    {
        using var scope = ServiceProvider.CreateScope();
        scope.ServiceProvider
             .GetRequiredService<IModuleManager>()
             .ShutdownModules(new ApplicationShutdownContext(scope.ServiceProvider));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="serviceProvider"></param>
    protected virtual void SetServiceProvider(IServiceProvider serviceProvider)
    {
        ServiceProvider = serviceProvider;
        ServiceProvider.GetRequiredService<ObjectAccessor<IServiceProvider>>().Value = ServiceProvider;
    }

    /// <summary>
    /// 
    /// </summary>
    protected virtual void InitializeModules()
    {
        using var scope = ServiceProvider.CreateScope();
        scope.ServiceProvider
             .GetRequiredService<IModuleManager>()
             .InitializeModules(new ApplicationInitializationContext(scope.ServiceProvider));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="services"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    protected virtual IReadOnlyList<IModuleDescriptor> LoadModules(IServiceCollection services, ApplicationCreationOptions options)
    {
        return services.GetSingletonInstance<IModuleLoader>()
                       .LoadModules(services, StartupModuleType);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <exception cref="Exception"></exception>
    protected virtual void ConfigureServices()
    {
        var context = new ServiceConfigurationContext(Services);
        Services.AddSingleton(context);

        foreach (var module in Modules)
        {
            if (module.Instance is ModuleContextBase moduleContext)
            {
                moduleContext.ConfigurationContext = context;
            }
        }

        foreach (var module in Modules)
        {
            try
            {
                module.Instance.PreparatoryServices(context);
            }
            catch (Exception exception)
            {
                throw new Exception($"An error occurred during {nameof(ModuleContextBase.PreparatoryServices)} phase of the module {module.Type.AssemblyQualifiedName}. See the inner exception for details.", exception);
            }
        }

        foreach (var module in Modules)
        {
            if (module.Instance is ModuleContextBase { AutomaticRegisterService: true })
            {
                Services.AddAssembly(module.Type.Assembly);
            }

            try
            {
                module.Instance.ConfigureServices(context);
            }
            catch (Exception exception)
            {
                throw new Exception($"An error occurred during {nameof(ModuleContextBase.ConfigureServices)} phase of the module {module.Type.AssemblyQualifiedName}. See the inner exception for details.", exception);
            }
        }

        foreach (var module in Modules)
        {
            if (module.Instance is ModuleContextBase moduleContext)
            {
                moduleContext.ConfigurationContext = null;
            }
        }
    }
}