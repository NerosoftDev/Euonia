using System.Reflection;
using System.Text;

namespace Nerosoft.Euonia.Business;

public abstract class RuleBase : IRuleBase
{
    protected RuleBase()
    {
        Name = GenerateName(GetType());
    }

    protected RuleBase(IPropertyInfo property)
    {
        Name = GenerateName(GetType(), property.Name);
        Property = property;
    }

    protected RuleBase(IPropertyInfo property, MemberInfo validationType)
    {
        Name = GenerateName(GetType(), property.Name, validationType.Name);
        Property = property;
    }

    public string Name { get; }
    public IPropertyInfo Property { get; }

    /// <summary>
    /// 
    /// </summary>
    public virtual List<IPropertyInfo> RelatedProperties { get; } = new();

    public int Priority { get; set; }

    /// <summary>
    /// Execute the rule check logic.
    /// </summary>
    /// <param name="context"></param>
    /// <param name="cancellationToken"></param>
    public virtual async Task ExecuteAsync(IRuleContext context, CancellationToken cancellationToken = default)
    {
        await Task.CompletedTask;
    }

    private static string GenerateName(Type ruleType, params string[] names)
    {
        var fullName = $"{ruleType.Namespace}.{ruleType.Name}";

        return GenerateName(fullName, names);
    }

    private static string GenerateName(string typeName, params string[] names)
    {
        var builder = new StringBuilder($"rule://{typeName}");
        foreach (var name in names)
        {
            builder.Append($"/{name}");
        }

        return builder.ToString().ToLowerInvariant();
    }
}