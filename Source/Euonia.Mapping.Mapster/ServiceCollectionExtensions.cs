using Mapster;
using MapsterMapper;
using Microsoft.Extensions.Options;
using Nerosoft.Euonia.Mapping;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// The <see cref="IServiceCollection"/> extensions to register object mapping provider.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds <see cref="IMapper"/> as object mapping provider.
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IServiceCollection AddMapster(this IServiceCollection services)
    {
        services.AddSingleton(provider =>
        {
            var options = provider.GetService<IOptions<MapsterOptions>>()?.Value;
            var configuration = options?.Configuration ?? TypeAdapterConfig.GlobalSettings;
            configuration.Apply(GetRegisters());
            return configuration;
        });
        services.AddScoped<IMapper, ServiceMapper>();
        return services;
    }

    private static IEnumerable<IRegister> GetRegisters()
    {
        yield return new ByteArrayToInt64Converter();
        yield return new DatetimeToTimestampConverter();
        yield return new DurationToTimespanConverter();
        yield return new TimespanToDurationConverter();
        yield return new TimestampToDatetimeConverter();
    }
}