using Nerosoft.Euonia.Caching;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extension methods for setting up caching services in an <see cref="T:Microsoft.Extensions.DependencyInjection.IServiceCollection" />.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds default cache manager.
    /// </summary>
    /// <param name="services"></param>
    /// <typeparam name="TComponent"></typeparam>
    /// <returns></returns>
    public static IServiceCollection AddDefaultCacheManager<TComponent>(this IServiceCollection services)
    {
        return services.AddSingleton<ICacheClock, DefaultCacheClock>()
                       .AddSingleton<ICacheHolder, DefaultCacheHolder>()
                       .AddSingleton<ICacheContextAccessor, DefaultCacheContextAccessor>()
                       .AddSingleton<IParallelCacheContext, DefaultParallelCacheContext>()
                       .AddSingleton<IAsyncTokenProvider, DefaultAsyncTokenProvider>()
                       .AddSingleton<ICacheSignal, DefaultCacheSignal>()
                       .AddSingleton<ICacheManager, DefaultCacheManager<TComponent>>();
    }
}