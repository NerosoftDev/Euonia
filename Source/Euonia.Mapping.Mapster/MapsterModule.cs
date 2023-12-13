using Microsoft.Extensions.DependencyInjection;
using Nerosoft.Euonia.Modularity;

namespace Nerosoft.Euonia.Mapping;

/// <summary>
/// The module used to configure mapster services.
/// </summary>
public class MapsterModule : ModuleContextBase
{
#if NET8_0_OR_GREATER
	private const string SERVICE_INJECTION_KEY = "mapster"; 
#endif

	/// <inheritdoc />
	public override void ConfigureServices(ServiceConfigurationContext context)
	{
		context.Services.AddMapster();
#if NET8_0_OR_GREATER
		context.Services.AddKeyedSingleton<ITypeAdapterFactory, MapsterTypeAdapterFactory>(SERVICE_INJECTION_KEY);
#else
		context.Services.AddSingleton<ITypeAdapterFactory, MapsterTypeAdapterFactory>();
#endif
	}

	/// <inheritdoc />
	public override void OnApplicationInitialization(ApplicationInitializationContext context)
	{
		ITypeAdapterFactory factory;
#if NET8_0_OR_GREATER
		factory = context.ServiceProvider.GetKeyedService<ITypeAdapterFactory>(SERVICE_INJECTION_KEY);
#else
		factory = context.ServiceProvider.GetService<ITypeAdapterFactory>();
#endif
		if (factory != null)
		{
			TypeAdapterFactory.SetCurrent(factory);
		}
	}
}