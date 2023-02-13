﻿using System.Reflection;

namespace Nerosoft.Euonia.Caching;

/// <summary>
/// Helper class to instantiate new <see cref="ICacheManager{TCacheValue}"/> instances from configuration.
/// </summary>
public static class CacheFactory
{
    /// <summary>
    /// <para>Instantiates a cache manager using the inline configuration defined by <paramref name="settings"/>.</para>
    /// <para>This Build method returns a <c>ICacheManager</c> with cache item type being <c>System.Object</c>.</para>
    /// </summary>
    /// <example>
    /// The following example show how to build a <c>CacheManagerConfiguration</c> and then
    /// using the <c>CacheFactory</c> to create a new cache manager instance.
    /// <code>
    /// <![CDATA[
    /// var cache = CacheFactory.Build("myCacheName", settings =>
    /// {
    ///    settings
    ///        .WithUpdateMode(CacheUpdateMode.Up)
    ///        .WithDictionaryHandle()
    ///            .EnablePerformanceCounters()
    ///            .WithExpiration(ExpirationMode.Sliding, TimeSpan.FromSeconds(10));
    /// });
    ///
    /// cache.Add("key", "value");
    /// ]]>
    /// </code>
    /// </example>
    /// <param name="cacheName">The name of the cache manager instance.</param>
    /// <param name="settings">
    /// The configuration. Use the settings element to configure the cache manager instance, add
    /// cache handles and also to configure the cache handles in a fluent way.
    /// </param>
    /// <returns>The cache manager instance with cache item type being <c>System.Object</c>.</returns>
    /// <seealso cref="ICacheManager{TCacheValue}"/>
    /// <exception cref="ArgumentNullException">
    /// Thrown if the <paramref name="cacheName"/> or <paramref name="settings"/> is null.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown on certain configuration errors related to the cache handles.
    /// </exception>
    public static ICacheManager<object> Build(string cacheName, Action<ConfigurationBuilderCachePart> settings) =>
        Build<object>(cacheName, settings);

    /// <summary>
    /// <para>Instantiates a cache manager using the inline configuration defined by <paramref name="settings"/>.</para>
    /// <para>This Build method returns a <see cref="ICacheManager{TCacheValue}"/> with cache item type being <see cref="object"/>.</para>
    /// </summary>
    /// <example>
    /// The following example show how to build a <c>CacheManagerConfiguration</c> and then
    /// using the <c>CacheFactory</c> to create a new cache manager instance.
    /// <code>
    /// <![CDATA[
    /// var cache = CacheFactory.Build(settings =>
    /// {
    ///    settings
    ///        .WithUpdateMode(CacheUpdateMode.Up)
    ///        .WithDictionaryHandle()
    ///            .EnablePerformanceCounters()
    ///            .WithExpiration(ExpirationMode.Sliding, TimeSpan.FromSeconds(10));
    /// });
    ///
    /// cache.Add("key", "value");
    /// ]]>
    /// </code>
    /// </example>
    /// <param name="settings">
    /// The configuration. Use the settings element to configure the cache manager instance, add
    /// cache handles and also to configure the cache handles in a fluent way.
    /// </param>
    /// <returns>The cache manager instance.</returns>
    /// <seealso cref="ICacheManager{TCacheValue}"/>
    /// <exception cref="ArgumentNullException">
    /// Thrown if <paramref name="settings"/> is null.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown on certain configuration errors related to the cache handles.
    /// </exception>
    public static ICacheManager<object> Build(Action<ConfigurationBuilderCachePart> settings) =>
        Build<object>(Guid.NewGuid().ToString("N"), settings);

    /// <summary>
    /// <para>Instantiates a cache manager using the inline configuration defined by <paramref name="settings"/>.</para>
    /// </summary>
    /// <example>
    /// The following example show how to build a <c>CacheManagerConfiguration</c> and then
    /// using the <c>CacheFactory</c> to create a new cache manager instance.
    /// <code>
    /// <![CDATA[
    /// var cache = CacheFactory.Build("myCacheName", settings =>
    /// {
    ///    settings
    ///        .WithUpdateMode(CacheUpdateMode.Up)
    ///        .WithDictionaryHandle()
    ///            .EnablePerformanceCounters()
    ///            .WithExpiration(ExpirationMode.Sliding, TimeSpan.FromSeconds(10));
    /// });
    ///
    /// cache.Add("key", "value");
    /// ]]>
    /// </code>
    /// </example>
    /// <param name="cacheName">The name of the cache manager instance.</param>
    /// <param name="settings">
    /// The configuration. Use the settings element to configure the cache manager instance, add
    /// cache handles and also to configure the cache handles in a fluent way.
    /// </param>
    /// <typeparam name="TCacheValue">The type of the cache item value.</typeparam>
    /// <returns>The cache manager instance with cache item type being <c>TCacheValue</c>.</returns>
    /// <seealso cref="ICacheManager{TCacheValue}"/>
    /// <exception cref="ArgumentNullException">
    /// Thrown if the <paramref name="cacheName"/> or <paramref name="settings"/> is null.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown on certain configuration errors related to the cache handles.
    /// </exception>
    public static ICacheManager<TCacheValue> Build<TCacheValue>(string cacheName, Action<ConfigurationBuilderCachePart> settings)
    {
        Check.EnsureNotNull(settings, nameof(settings));

        var part = new ConfigurationBuilderCachePart();
        settings(part);
        part.Configuration.Name = cacheName;
        return new BaseCacheManager<TCacheValue>(part.Configuration);
    }

    /// <summary>
    /// <para>Instantiates a cache manager using the inline configuration defined by <paramref name="settings"/>.</para>
    /// </summary>
    /// <example>
    /// The following example show how to build a <c>CacheManagerConfiguration</c> and then
    /// using the <c>CacheFactory</c> to create a new cache manager instance.
    /// <code>
    /// <![CDATA[
    /// var cache = CacheFactory.Build(settings =>
    /// {
    ///    settings
    ///        .WithUpdateMode(CacheUpdateMode.Up)
    ///        .WithDictionaryHandle()
    ///            .EnablePerformanceCounters()
    ///            .WithExpiration(ExpirationMode.Sliding, TimeSpan.FromSeconds(10));
    /// });
    ///
    /// cache.Add("key", "value");
    /// ]]>
    /// </code>
    /// </example>
    /// <param name="settings">
    /// The configuration. Use the settings element to configure the cache manager instance, add
    /// cache handles and also to configure the cache handles in a fluent way.
    /// </param>
    /// <typeparam name="TCacheValue">The type of the cache item value.</typeparam>
    /// <returns>The cache manager instance with cache item type being <c>TCacheValue</c>.</returns>
    /// <seealso cref="ICacheManager{TCacheValue}"/>
    /// <exception cref="ArgumentNullException">
    /// Thrown if <paramref name="settings"/> is null.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown on certain configuration errors related to the cache handles.
    /// </exception>
    public static ICacheManager<TCacheValue> Build<TCacheValue>(Action<ConfigurationBuilderCachePart> settings)
        => Build<TCacheValue>(Guid.NewGuid().ToString("N"), settings);

    /// <summary>
    /// Instantiates a cache manager using the given type and the inline configuration defined by <paramref name="settings"/>.
    /// Use this overload if you cannot invoke the generic method, for example in conjunction with dependency injection.
    /// </summary>
    /// <example>
    /// The following example show how to build a <c>CacheManagerConfiguration</c> and then
    /// using the <c>CacheFactory</c> to create a new cache manager instance.
    /// <code>
    /// <![CDATA[
    /// var cache = CacheFactory.Build(typeof(string), "myCacheName", settings =>
    /// {
    ///    settings
    ///        .WithUpdateMode(CacheUpdateMode.Up)
    ///        .WithDictionaryHandle()
    ///            .EnablePerformanceCounters()
    ///            .WithExpiration(ExpirationMode.Sliding, TimeSpan.FromSeconds(10));
    /// });
    ///
    /// ]]>
    /// </code>
    /// </example>
    /// <param name="cacheValueType">The type of the cache item value.</param>
    /// <param name="cacheName">The name of the cache manager instance.</param>
    /// <param name="settings">
    /// The configuration. Use the settings element to configure the cache manager instance, add
    /// cache handles and also to configure the cache handles in a fluent way.
    /// </param>
    /// <returns>The cache manager instance.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown if the <paramref name="cacheName"/> or <paramref name="settings"/> is null.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown on certain configuration errors related to the cache handles.
    /// </exception>
    public static object Build(Type cacheValueType, string cacheName, Action<ConfigurationBuilderCachePart> settings)
    {
        Check.EnsureNotNull(cacheValueType, nameof(cacheValueType));

        var factoryType = typeof(CacheFactory).GetTypeInfo();
        var buildMethod = factoryType.GetDeclaredMethods("Build").First(p => p.IsGenericMethod);
        var genericMethod = buildMethod.MakeGenericMethod(cacheValueType);
        return genericMethod.Invoke(null, new object[] { cacheName, settings });
    }

    /// <summary>
    /// Instantiates a cache manager using the given type and the inline configuration defined by <paramref name="settings"/>.
    /// Use this overload if you cannot invoke the generic method, for example in conjunction with dependency injection.
    /// </summary>
    /// <example>
    /// The following example show how to build a <c>CacheManagerConfiguration</c> and then
    /// using the <c>CacheFactory</c> to create a new cache manager instance.
    /// <code>
    /// <![CDATA[
    /// var cache = CacheFactory.Build(typeof(string), settings =>
    /// {
    ///    settings
    ///        .WithUpdateMode(CacheUpdateMode.Up)
    ///        .WithDictionaryHandle()
    ///            .EnablePerformanceCounters()
    ///            .WithExpiration(ExpirationMode.Sliding, TimeSpan.FromSeconds(10));
    /// });
    ///
    /// ]]>
    /// </code>
    /// </example>
    /// <param name="cacheValueType">The type of the cache item value.</param>
    /// <param name="settings">
    /// The configuration. Use the settings element to configure the cache manager instance, add
    /// cache handles and also to configure the cache handles in a fluent way.
    /// </param>
    /// <returns>The cache manager instance.</returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown if <paramref name="settings"/> is null.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown on certain configuration errors related to the cache handles.
    /// </exception>
    public static object Build(Type cacheValueType, Action<ConfigurationBuilderCachePart> settings)
        => Build(cacheValueType, Guid.NewGuid().ToString("N"), settings);

    /// <summary>
    /// <para>Instantiates a cache manager using the given <paramref name="configuration"/>.</para>
    /// </summary>
    /// <param name="cacheName">The name of the cache.</param>
    /// <param name="configuration">
    /// The configured which will be used to configure the cache manager instance.
    /// </param>
    /// <typeparam name="TCacheValue">The type of the cache item value.</typeparam>
    /// <returns>The cache manager instance.</returns>
    /// <see cref="ConfigurationBuilder"/>
    /// <exception cref="ArgumentNullException">
    /// Thrown if the <paramref name="configuration"/> is null.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown on certain configuration errors related to the cache handles.
    /// </exception>
    public static ICacheManager<TCacheValue> FromConfiguration<TCacheValue>(string cacheName, CacheManagerConfiguration configuration)
    {
        Check.EnsureNotNull(configuration, nameof(configuration));
        var cfg = configuration;
        cfg.Name = cacheName;
        return new BaseCacheManager<TCacheValue>(cfg);
    }

    /// <summary>
    /// <para>Instantiates a cache manager using the given <paramref name="configuration"/>.</para>
    /// </summary>
    /// <example>
    /// The following example show how to build a <c>CacheManagerConfiguration</c> and then
    /// using the <c>CacheFactory</c> to create a new cache manager instance.
    /// <code>
    /// <![CDATA[
    /// var managerConfiguration = ConfigurationBuilder.BuildConfiguration<object>(settings =>
    /// {
    ///     settings.WithUpdateMode(CacheUpdateMode.Up)
    ///         .WithDictionaryCacheHandle<object>>()
    ///             .EnablePerformanceCounters()
    ///             .WithExpiration(ExpirationMode.Sliding, TimeSpan.FromSeconds(10));
    /// });
    ///
    /// var cache = CacheFactory.FromConfiguration<object>(managerConfiguration);
    /// cache.Add("key", "value");
    /// ]]>
    /// </code>
    /// </example>
    /// <param name="configuration">
    /// The configured which will be used to configure the cache manager instance.
    /// </param>
    /// <typeparam name="TCacheValue">The type of the cache item value.</typeparam>
    /// <returns>The cache manager instance.</returns>
    /// <see cref="ConfigurationBuilder"/>
    /// <exception cref="ArgumentNullException">
    /// Thrown if the <paramref name="configuration"/> is null.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown on certain configuration errors related to the cache handles.
    /// </exception>
    public static ICacheManager<TCacheValue> FromConfiguration<TCacheValue>(CacheManagerConfiguration configuration)
        => FromConfiguration<TCacheValue>(Guid.NewGuid().ToString("N"), configuration);

    /// <summary>
    /// Instantiates a cache manager using the given <paramref name="cacheValueType"/> and <paramref name="configuration"/>.
    /// Use this overload only if you cannot use the generic overload. The return type will be <c>Object</c>.
    /// This method can be used for example in conjunction with dependency injection frameworks.
    /// </summary>
    /// <param name="cacheValueType">The type of the cache item value.</param>
    /// <param name="cacheName">The name of the cache.</param>
    /// <param name="configuration">
    /// The configured which will be used to configure the cache manager instance.
    /// </param>
    /// <returns>The cache manager instance.</returns>
    /// <see cref="ConfigurationBuilder"/>
    /// <exception cref="ArgumentNullException">
    /// Thrown if the <c>cacheValueType</c>, <c>cacheName</c> or <c>configuration</c> is null.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown on certain configuration errors related to the cache handles.
    /// </exception>
    public static object FromConfiguration(Type cacheValueType, string cacheName, CacheManagerConfiguration configuration)
    {
        Check.EnsureNotNull(cacheValueType, nameof(cacheValueType));
        Check.EnsureNotNull(configuration, nameof(configuration));

        var cfg = configuration;
        cfg.Name = cacheName;

        var type = typeof(BaseCacheManager<>).MakeGenericType(new[] { cacheValueType });
        return Activator.CreateInstance(type, new object[] { cfg });
    }

    /// <summary>
    /// Instantiates a cache manager using the given <paramref name="cacheValueType"/> and <paramref name="configuration"/>.
    /// Use this overload only if you cannot use the generic overload. The return type will be <c>Object</c>.
    /// This method can be used for example in conjunction with dependency injection frameworks.
    /// </summary>
    /// <param name="cacheValueType">The type of the cache item value.</param>
    /// <param name="configuration">
    /// The configured which will be used to configure the cache manager instance.
    /// </param>
    /// <returns>The cache manager instance.</returns>
    /// <see cref="ConfigurationBuilder"/>
    /// <exception cref="ArgumentNullException">
    /// Thrown if the <paramref name="cacheValueType"/> or <paramref name="configuration"/> are null.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// Thrown on certain configuration errors related to the cache handles.
    /// </exception>
    public static object FromConfiguration(Type cacheValueType, CacheManagerConfiguration configuration)
        => FromConfiguration(cacheValueType, Guid.NewGuid().ToString("N"), configuration);
}
