using Nerosoft.Euonia.Quartz;
using Quartz.Spi;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extension methods for setting up Quartz services in an <see cref="T:Microsoft.Extensions.DependencyInjection.IServiceCollection" />.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds Quartz services to the specified <see cref="T:Microsoft.Extensions.DependencyInjection.IServiceCollection" />.
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddQuartz(this IServiceCollection services)
    {
        services.AddTransient<IJobFactory, DefaultJobFactory>();
        services.AddHostedService<BackgroundJobService>();
        return services;
    }
}