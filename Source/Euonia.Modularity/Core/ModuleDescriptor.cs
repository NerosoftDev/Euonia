using System.Collections.Immutable;
using System.Reflection;

namespace Nerosoft.Euonia.Modularity;

/// <inheritdoc />
public class ModuleDescriptor : IModuleDescriptor
{
    private readonly List<IModuleDescriptor> _dependencies = new();

    /// <summary>
    /// 
    /// </summary>
    /// <param name="type"></param>
    /// <param name="instance"></param>
    public ModuleDescriptor(Type type, IModuleContext instance)
    {
        Type = type;
        Assembly = type.Assembly;
        Instance = instance;
    }

    /// <inheritdoc />
    public Type Type { get; }

    /// <inheritdoc />
    public Assembly Assembly { get; }

    /// <inheritdoc />
    public IModuleContext Instance { get; }

    /// <inheritdoc />
    public IReadOnlyList<IModuleDescriptor> Dependencies => _dependencies.ToImmutableList();

    /// <summary>
    /// 
    /// </summary>
    /// <param name="descriptor"></param>
    public void AddDependency(IModuleDescriptor descriptor)
    {
        _dependencies.AddIfNotContains(descriptor);
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"[ModuleDescriptor {Type.FullName}]";
    }
}
