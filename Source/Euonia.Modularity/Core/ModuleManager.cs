using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Nerosoft.Euonia.Dependency;

namespace Nerosoft.Euonia.Modularity;

public class ModuleManager : IModuleManager, ISingletonDependency
{
    private readonly IModuleContainer _moduleContainer;
    private readonly IEnumerable<IModuleLifecycle> _lifecycleContributors;

    public ModuleManager(IModuleContainer moduleContainer, IOptions<ModuleLifecycleOptions> options, IServiceProvider serviceProvider)
    {
        _moduleContainer = moduleContainer;

        _lifecycleContributors = options.Value
            .Lifecycle
            .Select(serviceProvider.GetRequiredService)
            .Cast<IModuleLifecycle>()
            .ToArray();
    }

    public void InitializeModules(ApplicationInitializationContext context)
    {
        foreach (var contributor in _lifecycleContributors)
        {
            foreach (var module in _moduleContainer.Modules)
            {
                try
                {
                    contributor.Initialize(context, module.Instance);
                }
                catch (Exception ex)
                {
                    throw new Exception($"An error occurred during the initialize {contributor.GetType().FullName} phase of the module {module.Type.AssemblyQualifiedName}: {ex.Message}. See the inner exception for details.", ex);
                }
            }
        }
    }

    public void ShutdownModules(ApplicationShutdownContext context)
    {
        var modules = _moduleContainer.Modules.Reverse().ToList();

        foreach (var contributor in _lifecycleContributors)
        {
            foreach (var module in modules)
            {
                try
                {
                    contributor.Shutdown(context, module.Instance);
                }
                catch (Exception ex)
                {
                    throw new Exception($"An error occurred during the shutdown {contributor.GetType().FullName} phase of the module {module.Type.AssemblyQualifiedName}: {ex.Message}. See the inner exception for details.", ex);
                }
            }
        }
    }
}
