using System.Reflection;

namespace Nerosoft.Euonia.Modularity;

public interface IAssemblyFinder
{
    IReadOnlyList<Assembly> Assemblies { get; }
}
