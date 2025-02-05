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

			if (options != null)
			{
				foreach (var (type, instance) in options.Profiles)
				{
					IRegister register;
					if (instance == null)
					{
						register = (IRegister)ActivatorUtilities.GetServiceOrCreateInstance(provider, type);
					}
					else
					{
						register = instance;
					}

					TypeAdapterConfig.GlobalSettings.Apply(register);
				}

				foreach (var configurator in options.Configuration)
				{
					configurator(TypeAdapterConfig.GlobalSettings);
				}
			}

			TypeAdapterConfig.GlobalSettings.Apply(GetRegisters());
			return TypeAdapterConfig.GlobalSettings;
		});
		services.AddScoped<IMapper, ServiceMapper>();
		return services;
	}

	private static IEnumerable<IRegister> GetRegisters()
	{
		yield return new ByteArrayToInt64Converter();
	}
}