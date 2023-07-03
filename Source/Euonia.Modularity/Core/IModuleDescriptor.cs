using System.Reflection;

namespace Nerosoft.Euonia.Modularity;

public interface IModuleDescriptor
{
    Type Type { get; }

    Assembly Assembly { get; }

    IModuleContext Instance { get; }

    IReadOnlyList<IModuleDescriptor> Dependencies { get;}
}
