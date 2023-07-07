using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Nerosoft.Euonia.Modularity;

namespace Nerosoft.Euonia.Threading.ZooKeeper;

internal class ZooKeeperSynchronizationModule : ModuleContextBase
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.TryAddSingleton(provider =>
        {
            var configuration = provider.GetRequiredService<IConfiguration>();
            var connection = configuration.GetValue<string>("ZooKeeperSynchronization:ConnectionString");
            var sessionTimeout = configuration.GetValue("ZooKeeperSynchronization:SessionTimeout", 0);
            var connectTimeout = configuration.GetValue("ZooKeeperSynchronization:ConnectTimeout", 0);

            return new ZooKeeperSynchronizationFactory(connection, builder =>
            {
                Check.Ensure(sessionTimeout, value => value > 0).Success(value => builder.SessionTimeout(TimeSpan.FromMilliseconds(value)));
                Check.Ensure(connectTimeout, value => value > 0).Success(value => builder.ConnectTimeout(TimeSpan.FromMilliseconds(value)));
            });
        });
    }
}

[DependsOn(typeof(ZooKeeperSynchronizationModule))]
public class ZooKeeperLockModule : ModuleContextBase
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddSingleton<ILockFactory>(provider => provider.GetRequiredService<ZooKeeperSynchronizationFactory>());
    }
}

[DependsOn(typeof(ZooKeeperSynchronizationModule))]
public class ZooKeeperSemaphoreModule : ModuleContextBase
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddSingleton<ISemaphoreFactory>(provider => provider.GetRequiredService<ZooKeeperSynchronizationFactory>());
    }
}