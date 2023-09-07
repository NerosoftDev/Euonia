using System.Reflection;

namespace Nerosoft.Euonia.Modularity;

/// <summary>
/// The module descriptor.
/// </summary>
public interface IModuleDescriptor
{
    /// <summary>
    /// Gets the module type.
    /// </summary>
    Type Type { get; }

    /// <summary>
    /// Gets the module assembly.
    /// </summary>
    Assembly Assembly { get; }

    /// <summary>
    /// Gets the module context instance.
    /// </summary>
    IModuleContext Instance { get; }

    /// <summary>
    /// Gets the dependencies of current module.
    /// </summary>
    IReadOnlyList<IModuleDescriptor> Dependencies { get;}
}
