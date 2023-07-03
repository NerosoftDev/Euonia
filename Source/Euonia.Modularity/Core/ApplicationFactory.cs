using Microsoft.Extensions.DependencyInjection;

namespace Nerosoft.Euonia.Modularity;

/// <summary>
/// 
/// </summary>
public static class ApplicationFactory
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="services"></param>
    /// <param name="optionsAction"></param>
    /// <typeparam name="TStartupModule"></typeparam>
    /// <returns></returns>
    public static IApplicationWithServiceProvider Create<TStartupModule>(IServiceCollection services, Action<ApplicationCreationOptions> optionsAction = null)
        where TStartupModule : IModuleContext
    {
        return Create(typeof(TStartupModule), services, optionsAction);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="startupModuleType"></param>
    /// <param name="services"></param>
    /// <param name="optionsAction"></param>
    /// <returns></returns>
    public static IApplicationWithServiceProvider Create(Type startupModuleType, IServiceCollection services, Action<ApplicationCreationOptions> optionsAction = null)
    {
        return new ApplicationWithServiceProvider(startupModuleType, services, optionsAction);
    }
}