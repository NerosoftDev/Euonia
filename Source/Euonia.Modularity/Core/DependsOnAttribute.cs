namespace Nerosoft.Euonia.Modularity;

/// <summary>
/// Represents that the class which this attribute is applied depends on the specified modules.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class DependsOnAttribute : Attribute
{
    /// <summary>
    /// Initialize a new instance which inherited <see cref="DependsOnAttribute"/>
    /// </summary>
    /// <param name="types"></param>
    public DependsOnAttribute(params Type[] types)
    {
        ModuleTypes = types ?? Array.Empty<Type>();
    }

    /// <summary>
    /// Gets the depended modules.
    /// </summary>
    public Type[] ModuleTypes { get; }
}
