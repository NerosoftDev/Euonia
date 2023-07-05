using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Nerosoft.Euonia.Modularity;

namespace Nerosoft.Euonia.Hosting;

public class HostingModule : ModuleContextBase
{
    /// <inheritdoc />
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddSingleton<IServiceAccessor, ServiceAccessor>();
        context.Services.AddSerilog();
        context.Services.AddScopeTransformation();
        context.Services.AddUserPrincipal();
        context.Services.AddObjectAccessor<IApplicationBuilder>();
    }

    /// <inheritdoc />
    public override void OnApplicationInitialization(ApplicationInitializationContext context)
    {
        base.OnApplicationInitialization(context);
        var app = context.GetApplicationBuilder();
        app.UseMiddleware<RequestTraceMiddleware>();
        app.Use(async (httpContext, next) =>
        {
            var accessor = httpContext.RequestServices.GetService<IServiceAccessor>();
            if (accessor != null)
            {
                accessor.ServiceProvider = httpContext.RequestServices;
            }

            await next();
        });
    }
}