using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Nerosoft.Euonia.Mapping.Tests.Profiles;

namespace Nerosoft.Euonia.Mapping.Tests;

[SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "<Pending>")]
[SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "<Pending>")]
public class Startup
{
	public void ConfigureHost(IHostBuilder hostBuilder)
	{
		hostBuilder.ConfigureAppConfiguration(builder =>
		           {
			           builder.AddJsonFile("appsettings.json");
		           })
		           .ConfigureServices((_, _) =>
		           {
			           // Register service here.
		           });
	}

	// ConfigureServices(IServiceCollection services)
	// ConfigureServices(IServiceCollection services, HostBuilderContext hostBuilderContext)
	// ConfigureServices(HostBuilderContext hostBuilderContext, IServiceCollection services)
	public void ConfigureServices(IServiceCollection services, HostBuilderContext hostBuilderContext)
	{
		services.AddSingleton<StringHelper>();
		services.Configure<MapsterOptions>(options =>
		{
			options.AddProfile<Profile1>();
			options.AddProfile<Profile2>();
			options.AddProfile<Profile3>();
		});
		services.AddMapster();
		services.AddSingleton<ITypeAdapterFactory, MapsterTypeAdapterFactory>();
	}

	//public void Configure(IServiceProvider applicationServices, IIdGenerator idGenerator)
	//{
	//}

	public void Configure(IServiceProvider applicationServices)
	{
		var factory = applicationServices.GetService<ITypeAdapterFactory>();
		if (factory != null)
		{
			TypeAdapterFactory.SetCurrent(factory);
		}
		//var config = applicationServices.GetService<IConfiguration>();
	}
}