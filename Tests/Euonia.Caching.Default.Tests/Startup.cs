﻿using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Nerosoft.Euonia.Caching.Tests;

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
                   .ConfigureServices((_, services) =>
                   {
                       services.AddDefaultCacheManager<Startup>();
                   });
    }

    // ConfigureServices(IServiceCollection services)
    // ConfigureServices(IServiceCollection services, HostBuilderContext hostBuilderContext)
    // ConfigureServices(HostBuilderContext hostBuilderContext, IServiceCollection services)
    public void ConfigureServices(IServiceCollection services, HostBuilderContext hostBuilderContext)
    {
    }

    //public void Configure(IServiceProvider applicationServices, IIdGenerator idGenerator)
    //{
    //}

    public void Configure(IServiceProvider applicationServices)
    {
        //var config = applicationServices.GetService<IConfiguration>();
    }
}