using System.Collections.Immutable;
using System.Reflection;

namespace Nerosoft.Euonia.Modularity;

/// <summary>
/// Finds all assemblies for the specified module container.
/// </summary>
public class AssemblyFinder : IAssemblyFinder
{
    private readonly IModuleContainer _moduleContainer;

    private readonly Lazy<IReadOnlyList<Assembly>> _assemblies;

    /// <summary>
    /// Initialize a new instance of <see cref="AssemblyFinder"/>.
    /// </summary>
    /// <param name="moduleContainer"></param>
    public AssemblyFinder(IModuleContainer moduleContainer)
    {
        _moduleContainer = moduleContainer;

        _assemblies = new Lazy<IReadOnlyList<Assembly>>(FindAll, LazyThreadSafetyMode.ExecutionAndPublication);
    }

    /// <inheritdoc />
    public IReadOnlyList<Assembly> Assemblies => _assemblies.Value;

    /// <summary>
    /// Finds all assemblies for the specified module container.
    /// </summary>
    /// <returns></returns>
    public IReadOnlyList<Assembly> FindAll()
    {
        var assemblies = new List<Assembly>();

        foreach (var module in _moduleContainer.Modules)
        {
            assemblies.Add(module.Type.Assembly);
        }

        return assemblies.Distinct().ToImmutableList();
    }
}
