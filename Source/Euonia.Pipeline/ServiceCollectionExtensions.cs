using Nerosoft.Euonia.Pipeline;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// 
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    /// <exception cref="NullReferenceException"></exception>
    public static IServiceCollection AddPipeline(this IServiceCollection services)
    {
        services.AddTransient<IPipeline, DefaultPipelineProvider>();
        services.AddTransient(typeof(IPipeline<,>), typeof(DefaultPipelineProvider<,>));

        services.AddTransient(provider =>
        {
            var pipeline = provider.GetService<IPipeline>();
            if (pipeline == null)
            {
                throw new NullReferenceException($"Can not resolve service {nameof(IPipeline)}");
            }

            var @delegate = pipeline.Build();
            return @delegate;
        });

        return services;
    }
}