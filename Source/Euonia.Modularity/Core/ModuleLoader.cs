using Microsoft.Extensions.DependencyInjection;

namespace Nerosoft.Euonia.Modularity;

public class ModuleLoader : IModuleLoader
{
    public IModuleDescriptor[] LoadModules(IServiceCollection services, Type startupModuleType)
    {
        var modules = GetDescriptors(services, startupModuleType);

        modules = SortByDependency(modules, startupModuleType);

        return modules.ToArray();
    }

    private List<IModuleDescriptor> GetDescriptors(IServiceCollection services, Type startupModuleType)
    {
        var modules = new List<ModuleDescriptor>();

        FillModules(modules, services, startupModuleType);
        SetDependencies(modules);

        return modules.Cast<IModuleDescriptor>().ToList();
    }

    protected virtual void FillModules(List<ModuleDescriptor> modules, IServiceCollection services, Type startupModuleType)
    {
        foreach (var moduleType in ModuleHelper.FindAllModuleTypes(startupModuleType))
        {
            modules.Add(CreateModuleDescriptor(services, moduleType));
        }

    }

    protected virtual void SetDependencies(List<ModuleDescriptor> modules)
    {
        foreach (var module in modules)
        {
            SetDependencies(modules, module);
        }
    }

    protected virtual List<IModuleDescriptor> SortByDependency(List<IModuleDescriptor> modules, Type startupModuleType)
    {
        var sortedModules = modules.SortByDependencies(m => m.Dependencies);
        sortedModules.MoveItem(m => m.Type == startupModuleType, modules.Count - 1);
        return sortedModules;
    }

    protected virtual ModuleDescriptor CreateModuleDescriptor(IServiceCollection services, Type moduleType)
    {
        return new ModuleDescriptor(moduleType, CreateAndRegisterModule(services, moduleType));
    }

    protected virtual IModuleContext CreateAndRegisterModule(IServiceCollection services, Type moduleType)
    {
        var module = (IModuleContext)Activator.CreateInstance(moduleType);
        services.AddSingleton(moduleType, module);
        return module;
    }

    protected virtual void SetDependencies(List<ModuleDescriptor> modules, ModuleDescriptor module)
    {
        foreach (var dependedModuleType in ModuleHelper.FindDependedModuleTypes(module.Type))
        {
            var dependedModule = modules.FirstOrDefault(m => m.Type == dependedModuleType);
            if (dependedModule == null)
            {
                throw new Exception($"Could not find a depended module {dependedModuleType.AssemblyQualifiedName} for {module.Type.AssemblyQualifiedName}");
            }
            module.AddDependency(dependedModule);
        }
    }
}
