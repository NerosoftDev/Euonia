using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Nerosoft.Euonia.Modularity;

namespace Nerosoft.Euonia.Hosting;

/// <summary>
/// The HostingModule class contains methods used to configure the hosting environment.
/// </summary>
public class HostingModule : ModuleContextBase
{
	/// <inheritdoc />
	public override void ConfigureServices(ServiceConfigurationContext context)
	{
		context.Services.TryAddScoped<DefaultRequestContextAccessor>();
		context.Services.TryAddScoped<DelegateRequestContextAccessor>(provider =>
		{
			return () => RequestContext.From(provider.GetService<IHttpContextAccessor>()?.HttpContext);
		});
		context.Services.AddScopeTransformation();
		context.Services.AddUserPrincipal();
		context.Services.AddObjectAccessor<IApplicationBuilder>();
		context.Services.AddTransient<ExceptionHandlingMiddleware>();
	}

	/// <inheritdoc />
	public override void OnApplicationInitialization(ApplicationInitializationContext context)
	{
		base.OnApplicationInitialization(context);
		var app = context.GetApplicationBuilder();

		if (app == null)
		{
			return;
		}

		app.UseMiddleware<RequestTraceMiddleware>();
		app.UseMiddleware<ExceptionHandlingMiddleware>();

		// Set up the ServiceProvider for IServiceAccessor.
		app.Use((httpContext, next) =>
		{
			var accessor = httpContext.RequestServices.GetService<IServiceAccessor>();
			if (accessor != null)
			{
				accessor.ServiceProvider = httpContext.RequestServices;
			}

			return next();
		});
	}
}