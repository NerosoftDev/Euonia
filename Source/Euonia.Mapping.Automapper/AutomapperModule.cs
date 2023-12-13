using Microsoft.Extensions.DependencyInjection;
using Nerosoft.Euonia.Modularity;

namespace Nerosoft.Euonia.Mapping;

/// <summary>
/// The module used to configure automapper services.
/// </summary>
public class AutomapperModule : ModuleContextBase
{
	private const string SERVICE_INJECTION_KEY = "automapper";

	/// <inheritdoc />
	public override void ConfigureServices(ServiceConfigurationContext context)
	{
		context.Services.AddAutomapper();
#if NET8_0_OR_GREATER
		context.Services.AddKeyedSingleton<ITypeAdapterFactory, AutomapperTypeAdapterFactory>(SERVICE_INJECTION_KEY);
#else
		context.Services.AddSingleton<ITypeAdapterFactory, AutomapperTypeAdapterFactory>();	
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