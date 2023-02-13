namespace Nerosoft.Euonia.Caching.Redis;

/// <summary>
/// Manages redis client configurations for the cache handle.
/// <para>
/// Configurations will be added by the cache configuration builder/factory or the configuration
/// loader. The cache handle will pick up the configuration matching the handle's name.
/// </para>
/// </summary>
public static class RedisConfigurations
{
    private static Dictionary<string, RedisConfiguration> _config;
    private static readonly object _configLock = new();

    private static Dictionary<string, RedisConfiguration> Configurations
    {
        get
        {
            if (_config == null)
            {
                lock (_configLock)
                {
                    _config ??= new Dictionary<string, RedisConfiguration>();
                }
            }

            return _config;
        }
    }

    /// <summary>
    /// Adds the configuration.
    /// </summary>
    /// <param name="configuration">The configuration.</param>
    /// <exception cref="ArgumentNullException">If configuration is null.</exception>
    public static void AddConfiguration(RedisConfiguration configuration)
    {
        lock (_configLock)
        {
            Check.EnsureNotNull(configuration, nameof(configuration));
            Check.EnsureNotNullOrWhiteSpace(configuration.Key, nameof(configuration.Key));

            if (!Configurations.ContainsKey(configuration.Key))
            {
                Configurations.Add(configuration.Key, configuration);
            }
        }
    }

    /// <summary>
    /// Gets the configuration.
    /// </summary>
    /// <param name="configurationName">The identifier.</param>
    /// <param name="connectionString"></param>
    /// <returns>The <c>RedisConfiguration</c>.</returns>
    /// <exception cref="ArgumentNullException">If id is null.</exception>
    /// <exception cref="InvalidOperationException">
    /// If no configuration was added for the id.
    /// </exception>
    public static RedisConfiguration GetConfiguration(string configurationName, string connectionString)
    {
        Check.EnsureNotNullOrWhiteSpace(configurationName, nameof(configurationName));

        if (!Configurations.ContainsKey(configurationName))
        {
            Check.EnsureNotNullOrWhiteSpace(connectionString, nameof(connectionString));

            // defaulting to database 0, no way to set it via connection strings atm.
            var configuration = new RedisConfiguration(configurationName, connectionString);
            AddConfiguration(configuration);
        }

        return Configurations[configurationName];
    }
}