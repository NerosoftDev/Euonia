using Castle.DynamicProxy;
using Microsoft.Extensions.DependencyInjection;
using Nerosoft.Euonia.Modularity;
using Nerosoft.Euonia.Pipeline;

namespace Nerosoft.Euonia.Application;

/// <inheritdoc />
public class ApplicationModule : ModuleContextBase
{
	/// <inheritdoc />
	public override void ConfigureServices(ServiceConfigurationContext context)
	{
		context.Services.AddTransient<IInterceptor, LoggingInterceptor>();
		context.Services.AddTransient<IInterceptor, AuthorizationInterceptor>();
		context.Services.AddTransient<IInterceptor, ValidationInterceptor>();
		context.Services.AddTransient<IInterceptor, TracingInterceptor>();
		context.Services.AddTransient<IInterceptor, LockInterceptor>();
		context.Services.AddSingleton(typeof(IPipelineBehavior<,>), typeof(MessageLoggingBehavior<,>));
		context.Services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
		context.Services.AddScoped(typeof(IPipelineBehavior<,>), typeof(AuthorizationBehavior<,>));
		context.Services.AddScoped(typeof(IPipelineBehavior<,>), typeof(UnitOfWorkPipelineBehavior<,>));
		context.Services.AddScoped(typeof(IUseCasePresenter<>), typeof(DefaultUseCasePresenter<>));
	}
}