using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Nerosoft.Euonia.Repository;
using Nerosoft.Euonia.Repository.Mongo;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="services"></param>
    /// <param name="options"></param>
    /// <param name="contextLifeTime"></param>
    /// <typeparam name="TContext"></typeparam>
    /// <returns></returns>
    public static IServiceCollection AddMongoRepository<TContext>(this IServiceCollection services, Action<MongoDbOptions> options, ServiceLifetime contextLifeTime = ServiceLifetime.Scoped)
        where TContext : MongoDbContext, IRepositoryContext
    {
        services.AddContextProvider()
                .AddUnitOfWork();
        services.AddMongoDbContext<TContext>(options, contextLifeTime)
                .AddMongoRepository(contextLifeTime);

        return services;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="services"></param>
    /// <param name="options"></param>
    /// <param name="contextLifeTime"></param>
    /// <typeparam name="TContextService"></typeparam>
    /// <typeparam name="TContextImplementation"></typeparam>
    /// <returns></returns>
    public static IServiceCollection AddMongoRepository<TContextService, TContextImplementation>(this IServiceCollection services, Action<MongoDbOptions> options, ServiceLifetime contextLifeTime = ServiceLifetime.Scoped)
        where TContextService : MongoDbContext, IRepositoryContext
        where TContextImplementation : TContextService
    {
        services.AddContextProvider()
                .AddUnitOfWork();
        services.AddMongoDbContext<TContextService, TContextImplementation>(options, contextLifeTime)
                .AddMongoRepository(contextLifeTime);

        return services;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="services"></param>
    /// <param name="contextLifeTime"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public static IServiceCollection AddMongoRepository(this IServiceCollection services, ServiceLifetime contextLifeTime = ServiceLifetime.Scoped)
    {
        switch (contextLifeTime)
        {
            case ServiceLifetime.Scoped:
                services.AddScoped(typeof(IRepository<,,>), typeof(MongoRepository<,,>));
                break;
            case ServiceLifetime.Singleton:
                services.AddSingleton(typeof(IRepository<,,>), typeof(MongoRepository<,,>));
                break;
            case ServiceLifetime.Transient:
                services.AddTransient(typeof(IRepository<,,>), typeof(MongoRepository<,,>));
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(contextLifeTime), contextLifeTime, null);
        }

        return services;
    }

    public static IServiceCollection AddMongoDbContext<TContextService, TContextImplementation>(this IServiceCollection services, Action<MongoDbOptions> configure, ServiceLifetime contextLifeTime = ServiceLifetime.Scoped)
        where TContextService : MongoDbContext, IRepositoryContext
        where TContextImplementation : TContextService
    {
        services.Configure(configure);
        services.AddSingleton(provider =>
        {
            var options = provider.GetService<IOptions<MongoDbOptions>>().Value;
            var client = new MongoClient(options.ConnectionString);
            return client.GetDatabase(options.Database);
        });

        switch (contextLifeTime)
        {
            case ServiceLifetime.Scoped:
                services.AddScoped<TContextService, TContextImplementation>();
                break;
            case ServiceLifetime.Singleton:
                services.AddSingleton<TContextService, TContextImplementation>();
                break;
            case ServiceLifetime.Transient:
                services.AddTransient<TContextService, TContextImplementation>();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(contextLifeTime), contextLifeTime, null);
        }

        return services;
    }

    public static IServiceCollection AddMongoDbContext<TContext>(this IServiceCollection services, Action<MongoDbOptions> configure, ServiceLifetime contextLifeTime = ServiceLifetime.Scoped)
        where TContext : MongoDbContext, IRepositoryContext
    {
        services.Configure(configure);
        services.AddSingleton(provider =>
        {
            var options = provider.GetService<IOptions<MongoDbOptions>>().Value;
            var client = new MongoClient(options.ConnectionString);
            return client.GetDatabase(options.Database);
        });

        switch (contextLifeTime)
        {
            case ServiceLifetime.Scoped:
                services.AddScoped<TContext>();
                break;
            case ServiceLifetime.Singleton:
                services.AddSingleton<TContext>();
                break;
            case ServiceLifetime.Transient:
                services.AddTransient<TContext>();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(contextLifeTime), contextLifeTime, null);
        }

        return services;
    }
}