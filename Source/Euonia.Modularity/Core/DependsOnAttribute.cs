namespace Nerosoft.Euonia.Modularity;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class DependsOnAttribute : Attribute
{
    public DependsOnAttribute(params Type[] types)
    {
        ModuleTypes = types ?? Array.Empty<Type>();
    }

    public Type[] ModuleTypes { get; }
}
