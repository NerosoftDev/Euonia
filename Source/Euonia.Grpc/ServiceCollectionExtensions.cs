using Grpc.AspNetCore.Server;
using Nerosoft.Euonia.Grpc;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// The gRPC service collection extensions.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Add gRPC service to specified <see cref="IServiceCollection"/>.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configureOptions"></param>
    /// <returns></returns>
    public static IServiceCollection AddGrpcService(this IServiceCollection services, Action<GrpcServiceOptions> configureOptions = null)
    {
        services.AddGrpc(options =>
        {
            options.MaxReceiveMessageSize = null;
            options.EnableDetailedErrors = true;
            options.Interceptors.Add<ExceptionHandlingInterceptor>();
            configureOptions?.Invoke(options);
        });
        services.AddGrpcReflection();
        return services;
    }
}