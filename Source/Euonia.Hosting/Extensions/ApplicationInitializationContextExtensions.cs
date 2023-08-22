using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nerosoft.Euonia.Dependency;
using Nerosoft.Euonia.Modularity;

namespace Nerosoft.Euonia.Hosting;

/// <summary>
/// The extension methods for <see cref="ApplicationInitializationContext"/>.
/// </summary>
public static class ApplicationInitializationContextExtensions
{
    /// <summary>
    /// Gets the <see cref="IApplicationBuilder"/> object.
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public static IApplicationBuilder GetApplicationBuilder(this ApplicationInitializationContext context)
    {
        return context.ServiceProvider.GetRequiredService<IObjectAccessor<IApplicationBuilder>>().Value;
    }

    /// <summary>
    /// Gets the <see cref="IWebHostEnvironment"/> object.
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public static IWebHostEnvironment GetEnvironment(this ApplicationInitializationContext context)
    {
        return context.ServiceProvider.GetRequiredService<IWebHostEnvironment>();
    }

    /// <summary>
    /// Gets the <see cref="IWebHostEnvironment"/> object.
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public static IWebHostEnvironment GetEnvironmentOrNull(this ApplicationInitializationContext context)
    {
        return context.ServiceProvider.GetService<IWebHostEnvironment>();
    }

    /// <summary>
    /// Gets the <see cref="IConfiguration"/> object.
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public static IConfiguration GetConfiguration(this ApplicationInitializationContext context)
    {
        return context.ServiceProvider.GetRequiredService<IConfiguration>();
    }
}
