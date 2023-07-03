﻿using Castle.DynamicProxy;
using Microsoft.Extensions.DependencyInjection;
using Nerosoft.Euonia.Modularity;

namespace Nerosoft.Euonia.Application;

public class ApplicationModule : ModuleContextBase
{
    /// <inheritdoc />
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddTransient<IInterceptor, LoggingInterceptor>();
        context.Services.AddTransient<IInterceptor, AuthorizationInterceptor>();
        context.Services.AddTransient<IInterceptor, ValidationInterceptor>();
        context.Services.AddTransient<IInterceptor, TracingInterceptor>();
        context.Services.AddScoped<IRequestContextAccessor, DelegateRequestContextAccessor>();
    }
}