using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Nerosoft.Euonia.Modularity;

namespace Nerosoft.Euonia.Mapping;

/// <summary>
/// The module used to configure mapster services.
/// </summary>
public class MapsterModule : ModuleContextBase
{
	private const string SERVICE_KEY = "mapster";

	/// <inheritdoc />
	public override void ConfigureServices(ServiceConfigurationContext context)
	{
		context.Services.AddMapster();
		context.Services.TryAddKeyedSingleton<ITypeAdapterFactory, MapsterTypeAdapterFactory>(SERVICE_KEY);
	}

	/// <inheritdoc />
	public override void OnApplicationInitialization(ApplicationInitializationContext context)
	{
		var factory = context.ServiceProvider.GetKeyedService<ITypeAdapterFactory>(SERVICE_KEY);
		if (factory != null)
		{
			TypeAdapterFactory.SetCurrent(factory);
		}
	}
}