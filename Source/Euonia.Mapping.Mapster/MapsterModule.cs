using Microsoft.Extensions.DependencyInjection;
using Nerosoft.Euonia.Modularity;

namespace Nerosoft.Euonia.Mapping;

/// <summary>
/// The module used to configure mapster services.
/// </summary>
public class MapsterModule : ModuleContextBase
{
	private const string SERVICE_INJECTION_KEY = "mapster";

	/// <inheritdoc />
	public override void ConfigureServices(ServiceConfigurationContext context)
	{
		context.Services.AddMapster();
		context.Services.AddKeyedSingleton<ITypeAdapterFactory, MapsterTypeAdapterFactory>(SERVICE_INJECTION_KEY);
	}

	/// <inheritdoc />
	public override void OnApplicationInitialization(ApplicationInitializationContext context)
	{
		var factory = context.ServiceProvider.GetKeyedService<ITypeAdapterFactory>(SERVICE_INJECTION_KEY);
		if (factory != null)
		{
			TypeAdapterFactory.SetCurrent(factory);
		}
	}
}