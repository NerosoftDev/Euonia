using Nerosoft.Euonia.Collections;

namespace Nerosoft.Euonia.Modularity;

public class ModuleLifecycleOptions
{
    public ITypeList<IModuleLifecycle> Lifecycle { get; }

    public ModuleLifecycleOptions()
    {
        Lifecycle = new TypeList<IModuleLifecycle>();
    }
}
