using Nerosoft.Euonia.Caching;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
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