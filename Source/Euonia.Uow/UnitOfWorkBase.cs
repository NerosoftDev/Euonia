using Microsoft.Extensions.DependencyInjection;

namespace Nerosoft.Euonia.Repository;

/// <summary>
/// 
/// </summary>
public abstract class UnitOfWorkBase : DisposableObject
{
    /// <summary>
    /// Gets the service provider.
    /// </summary>
    public abstract IServiceProvider ServiceProvider { get; }

    /// <summary>
    /// Gets a instance of <typeparamref name="TService"/> in current scope of unit of work.
    /// </summary>
    /// <typeparam name="TService">The type of service object to get.</typeparam>
    /// <returns></returns>
    public TService GetService<TService>()
        where TService : class
    {
        return ServiceProvider.GetService<TService>();
    }

    /// <summary>
    /// Gets instances of <typeparamref name="TService"/> in current scope of unit of work.
    /// </summary>
    /// <typeparam name="TService">The type of service object to get.</typeparam>
    /// <returns></returns>
    public IEnumerable<TService> GetServices<TService>()
        where TService : class
    {
        return ServiceProvider.GetServices<TService>();
    }

    /// <summary>
    /// Gets a instance of specified type in current scope of unit of work.
    /// </summary>
    /// <param name="serviceType">The type of service object to get.</param>
    /// <returns></returns>
    public object GetService(Type serviceType)
    {
        return ServiceProvider.GetService(serviceType);
    }

    /// <summary>
    /// Gets instances of specified type in current scope of unit of work.
    /// </summary>
    /// <param name="serviceType">The type of service object to get.</param>
    /// <returns></returns>
    public IEnumerable<object> GetServices(Type serviceType)
    {
        return ServiceProvider.GetServices(serviceType);
    }
}