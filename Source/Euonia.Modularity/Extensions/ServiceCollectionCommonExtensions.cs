using Nerosoft.Euonia.Modularity;
using System.Reflection;
using Castle.DynamicProxy;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// 
/// </summary>
public static class ServiceCollectionCommonExtensions
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="services"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static bool IsAdded<T>(this IServiceCollection services)
    {
        return services.IsAdded(typeof(T));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="services"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    public static bool IsAdded(this IServiceCollection services, Type type)
    {
        return services.Any(d => d.ServiceType == type);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="services"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T GetSingletonInstanceOrNull<T>(this IServiceCollection services)
    {
        return (T)services
                  .FirstOrDefault(d => d.ServiceType == typeof(T))
                  ?.ImplementationInstance;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="services"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public static T GetSingletonInstance<T>(this IServiceCollection services)
    {
        var service = services.GetSingletonInstanceOrNull<T>();
        if (service == null)
        {
            throw new InvalidOperationException("Could not find singleton service: " + typeof(T).AssemblyQualifiedName);
        }

        return service;
    }

    /// <summary>
    /// Resolves a dependency using given <see cref="IServiceCollection"/>.
    /// This method should be used only after dependency injection registration phase completed.
    /// </summary>
    internal static T GetService<T>(this IServiceCollection services)
    {
        return services
               .GetSingletonInstance<IModularityApplication>()
               .ServiceProvider
               .GetService<T>();
    }

    /// <summary>
    /// Resolves a dependency using given <see cref="IServiceCollection"/>.
    /// This method should be used only after dependency injection registration phase completed.
    /// </summary>
    internal static object GetService(this IServiceCollection services, Type type)
    {
        return services
               .GetSingletonInstance<IModularityApplication>()
               .ServiceProvider
               .GetService(type);
    }

    /// <summary>
    /// Resolves a dependency using given <see cref="IServiceCollection"/>.
    /// Throws exception if service is not registered.
    /// This method should be used only after dependency injection registration phase completed.
    /// </summary>
    public static T GetRequiredService<T>(this IServiceCollection services)
    {
        return services
               .GetSingletonInstance<IModularityApplication>()
               .ServiceProvider
               .GetRequiredService<T>();
    }

    /// <summary>
    /// Resolves a dependency using given <see cref="IServiceCollection"/>.
    /// Throws exception if service is not registered.
    /// This method should be used only after dependency injection registration phase completed.
    /// </summary>
    public static object GetRequiredService(this IServiceCollection services, Type type)
    {
        return services
               .GetSingletonInstance<IModularityApplication>()
               .ServiceProvider
               .GetRequiredService(type);
    }

    /// <summary>
    /// Returns a <see cref="Lazy{T}"/> to resolve a service from given <see cref="IServiceCollection"/>
    /// once dependency injection registration phase completed.
    /// </summary>
    public static Lazy<T> GetServiceLazy<T>(this IServiceCollection services)
    {
        return new Lazy<T>(services.GetService<T>, true);
    }

    /// <summary>
    /// Returns a <see cref="Lazy{T}"/> to resolve a service from given <see cref="IServiceCollection"/>
    /// once dependency injection registration phase completed.
    /// </summary>
    public static Lazy<object> GetServiceLazy(this IServiceCollection services, Type type)
    {
        return new Lazy<object>(() => services.GetService(type), true);
    }

    /// <summary>
    /// Returns a <see cref="Lazy{T}"/> to resolve a service from given <see cref="IServiceCollection"/>
    /// once dependency injection registration phase completed.
    /// </summary>
    public static Lazy<T> GetRequiredServiceLazy<T>(this IServiceCollection services)
    {
        return new Lazy<T>(services.GetRequiredService<T>, true);
    }

    /// <summary>
    /// Returns a <see cref="Lazy{T}"/> to resolve a service from given <see cref="IServiceCollection"/>
    /// once dependency injection registration phase completed.
    /// </summary>
    public static Lazy<object> GetRequiredServiceLazy(this IServiceCollection services, Type type)
    {
        return new Lazy<object>(() => services.GetRequiredService(type), true);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceProvider GetServiceProviderOrNull(this IServiceCollection services)
    {
        return services.GetObjectOrNull<IServiceProvider>();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="services"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static ObjectAccessor<T> TryAddObjectAccessor<T>(this IServiceCollection services)
    {
        if (services.Any(s => s.ServiceType == typeof(ObjectAccessor<T>)))
        {
            return services.GetSingletonInstance<ObjectAccessor<T>>();
        }

        return services.AddObjectAccessor<T>();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="services"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static ObjectAccessor<T> AddObjectAccessor<T>(this IServiceCollection services)
    {
        return services.AddObjectAccessor(new ObjectAccessor<T>());
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="services"></param>
    /// <param name="obj"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static ObjectAccessor<T> AddObjectAccessor<T>(this IServiceCollection services, T obj)
    {
        return services.AddObjectAccessor(new ObjectAccessor<T>(obj));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="services"></param>
    /// <param name="accessor"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public static ObjectAccessor<T> AddObjectAccessor<T>(this IServiceCollection services, ObjectAccessor<T> accessor)
    {
        if (services.Any(s => s.ServiceType == typeof(ObjectAccessor<T>)))
        {
            throw new Exception("An object accessor is registered before for type: " + typeof(T).AssemblyQualifiedName);
        }

        //Add to the beginning for fast retrieve
        services.Insert(0, ServiceDescriptor.Singleton(typeof(ObjectAccessor<T>), accessor));
        services.Insert(0, ServiceDescriptor.Singleton(typeof(IObjectAccessor<T>), accessor));

        return accessor;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="services"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T GetObjectOrNull<T>(this IServiceCollection services)
        where T : class
    {
        return services.GetSingletonInstanceOrNull<IObjectAccessor<T>>()?.Value;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="services"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public static T GetObject<T>(this IServiceCollection services)
        where T : class
    {
        return services.GetObjectOrNull<T>() ?? throw new Exception($"Could not find an object of {typeof(T).AssemblyQualifiedName} in services. Be sure that you have used AddObjectAccessor before!");
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="services"></param>
    /// <param name="registrar"></param>
    /// <returns></returns>
    public static IServiceCollection AddAutomaticRegistrar(this IServiceCollection services, IAutomaticRegistration registrar)
    {
        GetOrCreateRegistrars(services).Add(registrar);
        return services;
    }

    private static ConventionalRegistrarList GetOrCreateRegistrars(IServiceCollection services)
    {
        var conventionalRegistrars = services.GetSingletonInstanceOrNull<IObjectAccessor<ConventionalRegistrarList>>()?.Value;
        if (conventionalRegistrars == null)
        {
            conventionalRegistrars = new ConventionalRegistrarList { new DefaultAutomaticRegistration() };
            services.AddObjectAccessor(conventionalRegistrars);
        }

        {
        }
        return conventionalRegistrars;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="services"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static IServiceCollection AddAssemblyOf<T>(this IServiceCollection services)
    {
        return services.AddAssembly(typeof(T).GetTypeInfo().Assembly);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="services"></param>
    /// <param name="assembly"></param>
    /// <returns></returns>
    public static IServiceCollection AddAssembly(this IServiceCollection services, Assembly assembly)
    {
        foreach (var registrar in GetOrCreateRegistrars(services))
        {
            registrar.AddAssembly(services, assembly);
        }

        return services;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="services"></param>
    /// <param name="registrationAction"></param>
    public static void Registry(this IServiceCollection services, Action<IServiceRegistrationContext> registrationAction)
    {
        GetOrCreateRegistrationActions(services).Add(registrationAction);
    }

    private static ServiceRegistrationAction GetOrCreateRegistrationActions(IServiceCollection services)
    {
        var actionList = services.GetSingletonInstanceOrNull<IObjectAccessor<ServiceRegistrationAction>>()?.Value;
        if (actionList == null)
        {
            actionList = new ServiceRegistrationAction();
            services.AddObjectAccessor(actionList);
        }

        return actionList;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="services"></param>
    /// <param name="exposeAction"></param>
    public static void OnExposing(this IServiceCollection services, Action<IServiceExposingContext> exposeAction)
    {
        GetOrCreateExposing(services).Add(exposeAction);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static ServiceExposingAction GetExposingActions(this IServiceCollection services)
    {
        return GetOrCreateExposing(services);
    }

    private static ServiceExposingAction GetOrCreateExposing(IServiceCollection services)
    {
        var actionList = services.GetSingletonInstanceOrNull<IObjectAccessor<ServiceExposingAction>>()?.Value;
        if (actionList != null)
        {
            return actionList;
        }

        actionList = new ServiceExposingAction();
        services.AddObjectAccessor(actionList);

        return actionList;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="services"></param>
    /// <typeparam name="TService"></typeparam>
    /// <typeparam name="TImplementation"></typeparam>
    public static IServiceCollection AddProxiedScoped<TService, TImplementation>(this IServiceCollection services)
        where TService : class
        where TImplementation : class, TService
    {
        services.AddScoped<TImplementation>();
        services.AddScoped(typeof(TService), provider =>
        {
            var proxyGenerator = provider.GetRequiredService<ProxyGenerator>();
            var actual = provider.GetRequiredService<TImplementation>();
            var interceptors = provider.GetServices<IInterceptor>().ToArray();
            return proxyGenerator.CreateInterfaceProxyWithTarget(typeof(TService), actual, interceptors);
        });
        return services;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="services"></param>
    /// <param name="serviceType"></param>
    /// <param name="implementationType"></param>
    /// <returns></returns>
    public static IServiceCollection AddProxiedScoped(this IServiceCollection services, Type serviceType, Type implementationType)
    {
        services.AddScoped(implementationType);
        services.AddScoped(serviceType, provider =>
        {
            var proxyGenerator = provider.GetRequiredService<ProxyGenerator>();
            var actual = provider.GetRequiredService(implementationType);
            var interceptors = provider.GetServices<IInterceptor>().ToArray();
            return proxyGenerator.CreateInterfaceProxyWithTarget(serviceType, actual, interceptors);
        });
        return services;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="services"></param>
    /// <param name="interceptors"></param>
    /// <typeparam name="TService"></typeparam>
    /// <typeparam name="TImplementation"></typeparam>
    /// <returns></returns>
    public static IServiceCollection AddProxiedScoped<TService, TImplementation>(this IServiceCollection services, IEnumerable<IInterceptor> interceptors)
        where TService : class
        where TImplementation : class, TService
    {
        services.AddScoped<TImplementation>();
        services.AddScoped(typeof(TService), provider =>
        {
            var proxyGenerator = provider.GetRequiredService<ProxyGenerator>();
            var actual = provider.GetRequiredService<TImplementation>();
            return proxyGenerator.CreateInterfaceProxyWithTarget(typeof(TService), actual, interceptors?.ToArray());
        });
        return services;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="services"></param>
    /// <param name="interceptorTypes"></param>
    /// <typeparam name="TService"></typeparam>
    /// <typeparam name="TImplementation"></typeparam>
    /// <returns></returns>
    public static IServiceCollection AddProxiedScoped<TService, TImplementation>(this IServiceCollection services, IEnumerable<Type> interceptorTypes)
        where TService : class
        where TImplementation : class, TService
    {
        services.AddScoped<TImplementation>();
        services.AddScoped(typeof(TService), provider =>
        {
            var proxyGenerator = provider.GetRequiredService<ProxyGenerator>();
            var actual = provider.GetRequiredService<TImplementation>();
            var interceptors = interceptorTypes?.Select(type => (IInterceptor)ActivatorUtilities.GetServiceOrCreateInstance(provider, type)).ToArray();
            return proxyGenerator.CreateInterfaceProxyWithTarget(typeof(TService), actual, interceptors);
        });
        return services;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="services"></param>
    /// <typeparam name="TService"></typeparam>
    /// <typeparam name="TImplementation"></typeparam>
    public static IServiceCollection AddProxiedSingleton<TService, TImplementation>(this IServiceCollection services)
        where TService : class
        where TImplementation : class, TService
    {
        services.AddSingleton<TImplementation>();
        services.AddSingleton(typeof(TService), provider =>
        {
            var proxyGenerator = provider.GetRequiredService<ProxyGenerator>();
            var actual = provider.GetRequiredService<TImplementation>();
            var interceptors = provider.GetServices<IInterceptor>().ToArray();
            return proxyGenerator.CreateInterfaceProxyWithTarget(typeof(TService), actual, interceptors);
        });
        return services;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="services"></param>
    /// <param name="serviceType"></param>
    /// <param name="implementationType"></param>
    /// <returns></returns>
    public static IServiceCollection AddProxiedSingleton(this IServiceCollection services, Type serviceType, Type implementationType)
    {
        services.AddSingleton(implementationType);
        services.AddSingleton(serviceType, provider =>
        {
            var proxyGenerator = provider.GetRequiredService<ProxyGenerator>();
            var actual = provider.GetRequiredService(implementationType);
            var interceptors = provider.GetServices<IInterceptor>().ToArray();
            return proxyGenerator.CreateInterfaceProxyWithTarget(serviceType, actual, interceptors);
        });
        return services;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="services"></param>
    /// <param name="implementation"></param>
    /// <typeparam name="TService"></typeparam>
    /// <returns></returns>
    public static IServiceCollection AddProxiedSingleton<TService>(this IServiceCollection services, TService implementation)
    {
        services.AddSingleton(typeof(TService), provider =>
        {
            var proxyGenerator = provider.GetRequiredService<ProxyGenerator>();
            var interceptors = provider.GetServices<IInterceptor>().ToArray();
            return proxyGenerator.CreateInterfaceProxyWithTarget(typeof(TService), implementation, interceptors);
        });
        return services;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="services"></param>
    /// <param name="interceptors"></param>
    /// <typeparam name="TService"></typeparam>
    /// <typeparam name="TImplementation"></typeparam>
    /// <returns></returns>
    public static IServiceCollection AddProxiedSingleton<TService, TImplementation>(this IServiceCollection services, IEnumerable<IInterceptor> interceptors)
        where TService : class
        where TImplementation : class, TService
    {
        services.AddSingleton<TImplementation>();
        services.AddSingleton(typeof(TService), provider =>
        {
            var proxyGenerator = provider.GetRequiredService<ProxyGenerator>();
            var actual = provider.GetRequiredService<TImplementation>();
            return proxyGenerator.CreateInterfaceProxyWithTarget(typeof(TService), actual, interceptors?.ToArray());
        });
        return services;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="services"></param>
    /// <param name="interceptorTypes"></param>
    /// <typeparam name="TService"></typeparam>
    /// <typeparam name="TImplementation"></typeparam>
    /// <returns></returns>
    public static IServiceCollection AddProxiedSingleton<TService, TImplementation>(this IServiceCollection services, IEnumerable<Type> interceptorTypes)
        where TService : class
        where TImplementation : class, TService
    {
        services.AddSingleton<TImplementation>();
        services.AddSingleton(typeof(TService), provider =>
        {
            var proxyGenerator = provider.GetRequiredService<ProxyGenerator>();
            var actual = provider.GetRequiredService<TImplementation>();
            var interceptors = interceptorTypes?.Select(type => (IInterceptor)ActivatorUtilities.GetServiceOrCreateInstance(provider, type)).ToArray();
            return proxyGenerator.CreateInterfaceProxyWithTarget(typeof(TService), actual, interceptors);
        });
        return services;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="services"></param>
    /// <typeparam name="TService"></typeparam>
    /// <typeparam name="TImplementation"></typeparam>
    public static IServiceCollection AddProxiedTransient<TService, TImplementation>(this IServiceCollection services)
        where TService : class
        where TImplementation : class, TService
    {
        services.AddTransient<TImplementation>();
        services.AddTransient(typeof(TService), provider =>
        {
            var proxyGenerator = provider.GetRequiredService<ProxyGenerator>();
            var actual = provider.GetRequiredService<TImplementation>();
            var interceptors = provider.GetServices<IInterceptor>().ToArray();
            return proxyGenerator.CreateInterfaceProxyWithTarget(typeof(TService), actual, interceptors);
        });
        return services;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="services"></param>
    /// <param name="serviceType"></param>
    /// <param name="implementationType"></param>
    /// <returns></returns>
    public static IServiceCollection AddProxiedTransient(this IServiceCollection services, Type serviceType, Type implementationType)
    {
        services.AddTransient(implementationType);
        services.AddTransient(serviceType, provider =>
        {
            var proxyGenerator = provider.GetRequiredService<ProxyGenerator>();
            var actual = provider.GetRequiredService(implementationType);
            var interceptors = provider.GetServices<IInterceptor>().ToArray();
            return proxyGenerator.CreateInterfaceProxyWithTarget(serviceType, actual, interceptors);
        });
        return services;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="services"></param>
    /// <param name="implementation"></param>
    /// <typeparam name="TService"></typeparam>
    /// <returns></returns>
    public static IServiceCollection AddProxiedTransient<TService>(this IServiceCollection services, TService implementation)
    {
        services.AddTransient(typeof(TService), provider =>
        {
            var proxyGenerator = provider.GetRequiredService<ProxyGenerator>();
            var interceptors = provider.GetServices<IInterceptor>().ToArray();
            return proxyGenerator.CreateInterfaceProxyWithTarget(typeof(TService), implementation, interceptors);
        });
        return services;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="services"></param>
    /// <param name="interceptors"></param>
    /// <typeparam name="TService"></typeparam>
    /// <typeparam name="TImplementation"></typeparam>
    public static IServiceCollection AddProxiedTransient<TService, TImplementation>(this IServiceCollection services, IEnumerable<IInterceptor> interceptors)
        where TService : class
        where TImplementation : class, TService
    {
        services.AddTransient<TImplementation>();
        services.AddTransient(typeof(TService), provider =>
        {
            var proxyGenerator = provider.GetRequiredService<ProxyGenerator>();
            var actual = provider.GetRequiredService<TImplementation>();
            return proxyGenerator.CreateInterfaceProxyWithTarget(typeof(TService), actual, interceptors?.ToArray());
        });
        return services;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="services"></param>
    /// <param name="interceptorTypes"></param>
    /// <typeparam name="TService"></typeparam>
    /// <typeparam name="TImplementation"></typeparam>
    /// <returns></returns>
    public static IServiceCollection AddProxiedTransient<TService, TImplementation>(this IServiceCollection services, IEnumerable<Type> interceptorTypes)
        where TService : class
        where TImplementation : class, TService
    {
        services.AddTransient<TImplementation>();
        services.AddTransient(typeof(TService), provider =>
        {
            var proxyGenerator = provider.GetRequiredService<ProxyGenerator>();
            var actual = provider.GetRequiredService<TImplementation>();
            var interceptors = interceptorTypes?.Select(type => (IInterceptor)ActivatorUtilities.GetServiceOrCreateInstance(provider, type)).ToArray();
            return proxyGenerator.CreateInterfaceProxyWithTarget(typeof(TService), actual, interceptors);
        });
        return services;
    }
}