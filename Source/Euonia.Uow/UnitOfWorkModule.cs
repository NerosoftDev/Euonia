using Castle.DynamicProxy;
using Microsoft.Extensions.DependencyInjection;
using Nerosoft.Euonia.Modularity;

namespace Nerosoft.Euonia.Uow;

public class UnitOfWorkModule : ModuleContextBase
{
	public override void AheadConfigureServices(ServiceConfigurationContext context)
	{
		context.Services.AddTransient<IInterceptor, UnitOfWorkInterceptor>();
	}
}