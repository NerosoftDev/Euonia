using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Nerosoft.Euonia.Modularity;

namespace Nerosoft.Euonia.Mapping;

/// <summary>
/// The module used to configure automapper services.
/// </summary>
public class AutomapperModule : ModuleContextBase
{
	private const string SERVICE_KEY = "automapper";

	/// <inheritdoc />
	public override void ConfigureServices(ServiceConfigurationContext context)
	{
		context.Services.AddAutomapper();
		context.Services.TryAddKeyedSingleton<ITypeAdapterFactory, AutomapperTypeAdapterFactory>(SERVICE_KEY);
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