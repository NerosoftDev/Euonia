using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace Nerosoft.Euonia.Modularity;

/// <summary>
/// 
/// </summary>
public abstract class ModuleContextBase : IModuleContext
{
    /// <summary>
    /// Gets a value to indicate whether the service should registry automatically or not.
    /// </summary>
    protected internal virtual bool AutomaticRegisterService => true;

    /// <summary>
    /// 
    /// </summary>
    /// <exception cref="Exception"></exception>
    protected internal ServiceConfigurationContext ConfigurationContext
    {
        get
        {
            if (_configurationContext == null)
            {
                throw new Exception($"{nameof(ConfigurationContext)} is only available in the {nameof(ConfigureServices)} methods.");
            }

            return _configurationContext;
        }
        internal set => _configurationContext = value;
    }

    private ServiceConfigurationContext _configurationContext;

    /// <inheritdoc />
    public virtual void AheadConfigureServices(ServiceConfigurationContext context)
    {
    }

    /// <inheritdoc />
    public virtual void ConfigureServices(ServiceConfigurationContext context)
    {
    }

    /// <inheritdoc />
    public virtual void AfterConfigureServices(ServiceConfigurationContext context)
    {
    }

    /// <inheritdoc />
    public virtual void OnApplicationInitialization(ApplicationInitializationContext context)
    {
    }

    /// <inheritdoc />
    public virtual void OnApplicationShutdown(ApplicationShutdownContext context)
    {
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="configureOptions"></param>
    /// <typeparam name="TOptions"></typeparam>
    protected void Configure<TOptions>(Action<TOptions> configureOptions)
        where TOptions : class
    {
        ConfigurationContext.Services.Configure(configureOptions);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="name"></param>
    /// <param name="configureOptions"></param>
    /// <typeparam name="TOptions"></typeparam>
    protected void Configure<TOptions>(string name, Action<TOptions> configureOptions)
        where TOptions : class
    {
        ConfigurationContext.Services.Configure(name, configureOptions);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="configuration"></param>
    /// <typeparam name="TOptions"></typeparam>
    protected void Configure<TOptions>(IConfiguration configuration)
        where TOptions : class
    {
        ConfigurationContext.Services.Configure<TOptions>(configuration);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="configuration"></param>
    /// <param name="configureBinder"></param>
    /// <typeparam name="TOptions"></typeparam>
    protected void Configure<TOptions>(IConfiguration configuration, Action<BinderOptions> configureBinder)
        where TOptions : class
    {
        ConfigurationContext.Services.Configure<TOptions>(configuration, configureBinder);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="name"></param>
    /// <param name="configuration"></param>
    /// <typeparam name="TOptions"></typeparam>
    protected void Configure<TOptions>(string name, IConfiguration configuration)
        where TOptions : class
    {
        ConfigurationContext.Services.Configure<TOptions>(name, configuration);
    }
}