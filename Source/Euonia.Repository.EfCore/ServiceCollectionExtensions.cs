using Microsoft.EntityFrameworkCore;
using Nerosoft.Euonia.Repository;
using Nerosoft.Euonia.Repository.EfCore;

// ReSharper disable MemberCanBePrivate.Global

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// The extensions method to add the <see cref="IRepository{TEntity,TKey}"/> to <see cref="IServiceCollection"/>.
/// </summary>
public static class ServiceCollectionExtensions
{
	/// <summary>
	/// Adds ef core repository to <see cref="IServiceCollection"/>.
	/// </summary>
	/// <param name="services"></param>
	/// <param name="options"></param>
	/// <param name="contextLifeTime"></param>
	/// <typeparam name="TContext"></typeparam>
	/// <returns></returns>
	public static IServiceCollection AddEfCoreRepository<TContext>(this IServiceCollection services, Action<DbContextOptionsBuilder> options, ServiceLifetime contextLifeTime = ServiceLifetime.Scoped)
		where TContext : DbContext, IRepositoryContext
	{
		services.AddContextProvider()
		        .AddUnitOfWork();
		services.AddDbContext<TContext>(options, contextLifeTime)
		        .AddEfCoreRepository(contextLifeTime);

		return services;
	}

	/// <summary>
	/// Adds ef core repository to <see cref="IServiceCollection"/>.
	/// </summary>
	/// <param name="services"></param>
	/// <param name="options"></param>
	/// <param name="contextLifeTime"></param>
	/// <typeparam name="TContextService"></typeparam>
	/// <typeparam name="TContextImplementation"></typeparam>
	/// <returns></returns>
	public static IServiceCollection AddEfCoreRepository<TContextService, TContextImplementation>(this IServiceCollection services, Action<DbContextOptionsBuilder> options, ServiceLifetime contextLifeTime = ServiceLifetime.Scoped)
		where TContextService : DbContext, IRepositoryContext
		where TContextImplementation : TContextService
	{
		services.AddContextProvider()
		        .AddUnitOfWork();
		services.AddDbContext<TContextService, TContextImplementation>(options, contextLifeTime)
		        .AddEfCoreRepository(contextLifeTime);

		return services;
	}

	/// <summary>
	/// Adds ef core repository to <see cref="IServiceCollection"/>.
	/// </summary>
	/// <param name="services"></param>
	/// <param name="options"></param>
	/// <param name="contextLifeTime"></param>
	/// <typeparam name="TContext"></typeparam>
	/// <returns></returns>
	public static IServiceCollection AddEfCoreRepository<TContext>(this IServiceCollection services, Action<IServiceProvider, DbContextOptionsBuilder> options, ServiceLifetime contextLifeTime = ServiceLifetime.Scoped)
		where TContext : DbContext, IRepositoryContext
	{
		services.AddContextProvider()
		        .AddUnitOfWork();
		services.AddDbContext<DbContext, TContext>(options, contextLifeTime)
		        .AddEfCoreRepository(contextLifeTime);

		return services;
	}

	/// <summary>
	/// Adds ef core repository to <see cref="IServiceCollection"/>.
	/// </summary>
	/// <param name="services"></param>
	/// <param name="options"></param>
	/// <param name="contextLifeTime"></param>
	/// <typeparam name="TContextService"></typeparam>
	/// <typeparam name="TContextImplementation"></typeparam>
	/// <returns></returns>
	public static IServiceCollection AddEfCoreRepository<TContextService, TContextImplementation>(this IServiceCollection services, Action<IServiceProvider, DbContextOptionsBuilder> options, ServiceLifetime contextLifeTime = ServiceLifetime.Scoped)
		where TContextService : DbContext, IRepositoryContext
		where TContextImplementation : TContextService
	{
		services.AddContextProvider()
		        .AddUnitOfWork();
		services.AddDbContext<TContextService, TContextImplementation>(options, contextLifeTime)
		        .AddEfCoreRepository(contextLifeTime);

		return services;
	}

	/// <summary>
	/// Adds ef core repository to <see cref="IServiceCollection"/>.
	/// </summary>
	/// <param name="services"></param>
	/// <param name="contextLifeTime"></param>
	/// <returns></returns>
	/// <exception cref="ArgumentOutOfRangeException"></exception>
	public static IServiceCollection AddEfCoreRepository(this IServiceCollection services, ServiceLifetime contextLifeTime = ServiceLifetime.Scoped)
	{
		switch (contextLifeTime)
		{
			case ServiceLifetime.Scoped:
				services.AddScoped(typeof(IRepository<,,>), typeof(EfCoreRepository<,,>));
				break;
			case ServiceLifetime.Singleton:
				services.AddSingleton(typeof(IRepository<,,>), typeof(EfCoreRepository<,,>));
				break;
			case ServiceLifetime.Transient:
				services.AddTransient(typeof(IRepository<,,>), typeof(EfCoreRepository<,,>));
				break;
			default:
				throw new ArgumentOutOfRangeException(nameof(contextLifeTime), contextLifeTime, null);
		}

		return services;
	}
}