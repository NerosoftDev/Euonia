using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Nerosoft.Euonia.Bus.InMemory;

namespace Nerosoft.Euonia.Bus.Tests;

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
		services.AddServiceBus(config =>
		{
			config.RegisterHandlers(Assembly.GetExecutingAssembly());
			config.SetConventions(builder =>
			{
				builder.Add<DefaultMessageConvention>();
				builder.Add<AttributeMessageConvention>();
				builder.EvaluateQueue(t => t.Name.EndsWith("Command"));
				builder.EvaluateTopic(t => t.Name.EndsWith("Event"));
				builder.EvaluateRequest(t => t.Name.EndsWith("Request"));
			});
			config.UseInMemory(options =>
			{
				options.MultipleSubscriberInstance = false;
			});
		});
	}

	//public void Configure(IServiceProvider applicationServices, IIdGenerator idGenerator)
	//{
	//  InitData();
	//}

	public void Configure(IServiceProvider applicationServices)
	{
		//var config = applicationServices.GetService<IConfiguration>();
	}
}