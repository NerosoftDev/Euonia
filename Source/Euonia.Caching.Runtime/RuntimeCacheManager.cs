using System.Collections.Concurrent;

namespace Nerosoft.Euonia.Caching.Runtime;

internal class RuntimeCacheManager
{
    private readonly ConcurrentDictionary<Type, object> _instances = new();

    private readonly CacheManagerConfiguration _configuration;

    public RuntimeCacheManager(RuntimeCacheOptions options)
    {
        var configuration = ConfigurationBuilder.BuildConfiguration(settings =>
        {
            settings.WithUpdateMode(options.UpdateMode)
                    .WithMaxRetries(options.MaxRetries)
                    .WithRetryTimeout(options.RetryTimeout)
                    .WithRuntimeCacheHandle(options.InstanceName, options)
                    .WithExpiration(CacheExpirationMode.Default, options.Expires ?? TimeSpan.MaxValue);
        });

        _configuration = configuration;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public ICacheManager<T> Instance<T>()
    {
        return (ICacheManager<T>)_instances.GetOrAdd(typeof(T), _ => CacheFactory.FromConfiguration<T>(_configuration));
    }
}
