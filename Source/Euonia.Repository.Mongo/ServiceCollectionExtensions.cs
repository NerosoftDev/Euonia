﻿using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Nerosoft.Euonia.Repository;
using Nerosoft.Euonia.Repository.Mongo;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// The extension methods for <see cref="IServiceCollection"/>.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds the mongo repository.
    /// </summary>
    /// <param name="services">The service collection instance.</param>
    /// <param name="configure">The mongo database options configure action.</param>
    /// <param name="contextLifeTime"></param>
    /// <typeparam name="TContext"></typeparam>
    /// <returns></returns>
    public static IServiceCollection AddMongoRepository<TContext>(this IServiceCollection services, Action<MongoDbOptions> configure, ServiceLifetime contextLifeTime = ServiceLifetime.Scoped)
        where TContext : MongoDbContext, IRepositoryContext
    {
        services.AddContextProvider()
                .AddUnitOfWork();
        services.AddMongoDbContext<TContext>(configure, contextLifeTime)
                .AddMongoRepository(contextLifeTime);

        return services;
    }

    /// <summary>
    /// Adds the mongo repository.
    /// </summary>
    /// <param name="services">The service collection instance.</param>
    /// <param name="configure">The mongo database options configure action.</param>
    /// <param name="contextLifeTime"></param>
    /// <typeparam name="TContextService"></typeparam>
    /// <typeparam name="TContextImplementation"></typeparam>
    /// <returns></returns>
    public static IServiceCollection AddMongoRepository<TContextService, TContextImplementation>(this IServiceCollection services, Action<MongoDbOptions> configure, ServiceLifetime contextLifeTime = ServiceLifetime.Scoped)
        where TContextService : MongoDbContext, IRepositoryContext
        where TContextImplementation : TContextService
    {
        services.AddContextProvider()
                .AddUnitOfWork();
        services.AddMongoDbContext<TContextService, TContextImplementation>(configure, contextLifeTime)
                .AddMongoRepository(contextLifeTime);

        return services;
    }

    /// <summary>
    /// Adds the mongo repository.
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

    /// <summary>
    /// Adds the mongo database context.
    /// </summary>
    /// <param name="services">The service collection instance.</param>
    /// <param name="configure">The mongo database options configure action.</param>
    /// <param name="contextLifeTime"></param>
    /// <typeparam name="TContextService"></typeparam>
    /// <typeparam name="TContextImplementation"></typeparam>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
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

    /// <summary>
    /// Adds the mongo database context.
    /// </summary>
    /// <param name="services">The service collection instance.</param>
    /// <param name="configure">The mongo database options configure action.</param>
    /// <param name="contextLifeTime"></param>
    /// <typeparam name="TContext"></typeparam>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
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