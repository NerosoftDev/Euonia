using System.Reflection;
using Castle.DynamicProxy;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Nerosoft.Euonia.Application;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// The extension methods to register application services to <see cref="IServiceCollection"/>.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Register service context.
    /// </summary>
    /// <param name="services"></param>
    /// <typeparam name="TService"></typeparam>
    public static void Register<TService>(this IServiceCollection services)
        where TService : class, IServiceContext, new()
    {
        var context = new TService();
        context.ConfigureServices(services);

        if (context.AutoRegisterApplicationService)
        {
            var assembly = Assembly.GetAssembly(typeof(TService));

            services.AddApplicationService(assembly);
        }

        services.TryAddSingleton<IServiceContext>(_ => context);
    }

    /// <summary>
    /// Register application service of module to <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> instance of current application.</param>
    /// <param name="assembly">The assembly which contains application services.</param>
    /// <returns></returns>
    public static IServiceCollection AddApplicationService(this IServiceCollection services, Assembly assembly)
    {
        if (assembly == null)
        {
            return services;
        }

        var definedTypes = assembly.DefinedTypes;
        services.AddApplicationService(definedTypes);
        return services;
    }

    /// <summary>
    /// Register application services of module to <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> instance of current application.</param>
    /// <param name="definedTypes">The application service types.</param>
    /// <returns></returns>
    /// <remarks>The application service type should inherits from <see cref="IApplicationService"/>.</remarks>
    public static IServiceCollection AddApplicationService(this IServiceCollection services, IEnumerable<Type> definedTypes)
    {
        if (!definedTypes.Any())
        {
            return services;
        }

        var types = definedTypes.Where(type => type.IsClass && !type.IsAbstract && typeof(IApplicationService).IsAssignableFrom(type));

        foreach (var implementationType in types)
        {
            services.AddTransient(implementationType);

            var interfaces = implementationType.GetInterfaces();

            if (interfaces.Length == 0)
            {
                continue;
            }

            foreach (var serviceType in interfaces)
            {
                services.TryAddTransient(serviceType, provider =>
                {
                    var instance = provider.GetRequiredService(implementationType);
                    if (instance is IHasLazyServiceProvider service)
                    {
                        var lazyServiceProvider = provider.GetService<ILazyServiceProvider>() ?? new LazyServiceProvider(provider);
                        service.LazyServiceProvider = lazyServiceProvider;
                    }

                    var proxyGenerator = provider.GetRequiredService<ProxyGenerator>();
                    var interceptors = provider.GetServices<IInterceptor>().ToArray();
                    return proxyGenerator.CreateInterfaceProxyWithTarget(serviceType, instance, interceptors);
                });
            }
        }

        return services;
    }
}