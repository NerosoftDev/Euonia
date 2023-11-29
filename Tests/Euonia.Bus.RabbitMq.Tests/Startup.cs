﻿using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Nerosoft.Euonia.Bus.RabbitMq;

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
		var preventUnitTest = hostBuilderContext.Configuration.GetValue<bool>("PreventRunTests");
		if (!preventUnitTest)
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
				});
				config.UseRabbitMq(options =>
				{
					options.Connection = "amqp://127.0.0.1";
					options.QueueName = "nerosoft.euonia.test.command";
					options.TopicName = "nerosoft.euonia.test.event";
					options.ExchangeName = $"nerosoft.euonia.test.exchange.{options.ExchangeType}";
					options.RoutingKey = "*";
				});
			});
		}
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
