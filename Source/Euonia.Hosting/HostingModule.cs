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
		context.Services.TryAddScoped<RequestContextAccessor>(provider => GetRequestContext);
		context.Services.AddSingleton<IServiceAccessor, ServiceAccessor>();
		context.Services.TryAddScoped<IRequestContextAccessor, DelegateRequestContextAccessor>();
		context.Services.AddSerilog();
		context.Services.AddScopeTransformation();
		context.Services.AddUserPrincipal();
		context.Services.AddObjectAccessor<IApplicationBuilder>();
		context.Services.AddTransient<ExceptionHandlingMiddleware>();

		static RequestContext GetRequestContext(IServiceProvider provider)
		{
			var context = provider.GetService<IHttpContextAccessor>()?.HttpContext;
			if (context == null)
			{
				return null;
			}
			return new RequestContext
			{
				RequestHeaders = (IDictionary<string, string>)(context?.Request?.Headers),
				ConnectionId = context?.Connection?.Id,

			};
		}
	}

	/// <inheritdoc />
	public override void OnApplicationInitialization(ApplicationInitializationContext context)
	{
		base.OnApplicationInitialization(context);
		var app = context.GetApplicationBuilder();
		app.UseMiddleware<RequestTraceMiddleware>();
		app.UseMiddleware<ExceptionHandlingMiddleware>();

		// Setup the ServiceProvider for IServiceAccessor.
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