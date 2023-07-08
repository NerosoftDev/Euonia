namespace Nerosoft.Euonia.Business;

/// <summary>
/// Contains information about the result of a rule.
/// </summary>
public class RuleResult
{
    public RuleResult(string ruleName)
    {
        RuleName = ruleName;
        Success = true;
    }

    public RuleResult(string ruleName, string description, RuleSeverity severity)
    {
        RuleName = ruleName;
        Success = string.IsNullOrEmpty(description);
        Description = description;
        Severity = severity;
    }

    /// <summary>
    /// Gets a value indicating whether the rule was successful.
    /// </summary>
    public bool Success { get; private set; }

    /// <summary>
    /// Gets a human-readable description of why the rule failed.
    /// </summary>
    public string Description { get; private set; }

    /// <summary>
    /// Gets or sets the severity of a failed rule.
    /// </summary>
    public RuleSeverity Severity { get; set; }

    /// <summary>
    /// <para>Gets the unique name of the rule.</para>
    /// <para>获取规则唯一名称</para>
    /// </summary>
    public string RuleName { get; private set; }

    /// <summary>
    /// <para>Gets or sets a list of properties that were affected by the rule.</para>
    /// <para>获取或设置受规则影响的属性列表</para>
    /// </summary>
    public IList<IPropertyInfo> Properties { get; set; }
}