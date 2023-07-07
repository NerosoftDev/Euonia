using Microsoft.Extensions.DependencyInjection;
using Nerosoft.Euonia.Modularity;

namespace Nerosoft.Euonia.Threading.Azure;

internal class AzureSynchronizationModule : ModuleContextBase
{
}

[DependsOn(typeof(AzureSynchronizationModule))]
public class AzureLockModule : ModuleContextBase
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddSingleton<ILockFactory, AzureSynchronizationFactory>();
    }
}