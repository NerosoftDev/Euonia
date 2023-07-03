namespace Nerosoft.Euonia.Caching.Redis;

internal class RedisCacheManager
{
    private readonly IDictionary<Type, object> _instances = new Dictionary<Type, object>();

    private readonly CacheManagerConfiguration _configuration;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="options"></param>
    public RedisCacheManager(RedisCacheOptions options)
    {
        var configuration = ConfigurationBuilder.BuildConfiguration(settings =>
        {
            //var updateMode = (CacheUpdateMode)Enum.Parse(typeof(CacheUpdateMode), options.UpdateMode);
            settings.WithUpdateMode(options.UpdateMode)
                    .WithMaxRetries(options.MaxRetries)
                    .WithRetryTimeout(options.RetryTimeout)
                    .WithRedisBackplane("redisConnection")
                    .WithRedisConfiguration("redisConnection", options.ConnectionString, options.Database)
                    .WithRedisCacheHandle("redisConnection")
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
