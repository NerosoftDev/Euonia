using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Nerosoft.Euonia.Application;

public interface IServiceContext
{
    /// <summary>
    /// Gets the service assembly.
    /// </summary>
    Assembly Assembly { get; }

    /// <summary>
    /// Gets a value that indicates whether the application service should be automatically registered or not.
    /// </summary>
    bool AutoRegisterApplicationService { get; }

    /// <summary>
    /// Configure the services that are required by the application.
    /// </summary>
    /// <param name="services"></param>
    void ConfigureServices(IServiceCollection services);
}