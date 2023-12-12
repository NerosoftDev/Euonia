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
		context.Services.AddKeyedSingleton<ITypeAdapterFactory, AutomapperTypeAdapterFactory>(SERVICE_INJECTION_KEY);
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