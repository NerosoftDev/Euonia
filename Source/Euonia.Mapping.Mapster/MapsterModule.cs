﻿using Microsoft.Extensions.DependencyInjection;
using Nerosoft.Euonia.Modularity;

namespace Nerosoft.Euonia.Mapping;

/// <summary>
/// The module used to configure mapster services.
/// </summary>
public class MapsterModule : ModuleContextBase
{
	/// <inheritdoc />
	public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddMapster();
        context.Services.AddSingleton<ITypeAdapterFactory, MapsterTypeAdapterFactory>();
    }

	/// <inheritdoc />
	public override void OnApplicationInitialization(ApplicationInitializationContext context)
    {
        var factory = context.ServiceProvider.GetService<ITypeAdapterFactory>();
        if (factory != null)
        {
            TypeAdapterFactory.SetCurrent(factory);
        }
    }
}