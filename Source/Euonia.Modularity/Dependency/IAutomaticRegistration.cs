using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Nerosoft.Euonia.Modularity;

/// <summary>
/// Defines a interface for automatic registration.
/// </summary>
public interface IAutomaticRegistration
{
    /// <summary>
    /// Add types from the given assembly to the service collection.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="assembly"></param>
    void AddAssembly(IServiceCollection services, Assembly assembly);

    /// <summary>
    /// Add types to the service collection.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="types"></param>
    void AddTypes(IServiceCollection services, params Type[] types);

    /// <summary>
    /// Add type to the service collection.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="type"></param>
    void AddType(IServiceCollection services, Type type);
}
