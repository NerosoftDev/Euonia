using System.Reflection;

namespace Nerosoft.Euonia.Modularity;

/// <summary>
/// Finds the assemblies used by the application.
/// </summary>
public interface IAssemblyFinder
{
    /// <summary>
    /// Gets the assemblies.
    /// </summary>
    /// <returns>An immutable list containing the assemblies used by the application.</returns>
    IReadOnlyList<Assembly> Assemblies { get; }
}
