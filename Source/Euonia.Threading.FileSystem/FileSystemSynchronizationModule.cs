using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Nerosoft.Euonia.Modularity;

namespace Nerosoft.Euonia.Threading.FileSystem;

internal class FileSystemSynchronizationModule : ModuleContextBase
{
}

[DependsOn(typeof(FileSystemSynchronizationModule))]
public class FileSystemLockModule : ModuleContextBase
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddSingleton<ILockFactory>(provider =>
        {
            var options = provider.GetService<IOptions<FileSynchronizationOptions>>().Value;
            return new FileSynchronizationFactory(new DirectoryInfo(options.Directory));
        });
    }
}