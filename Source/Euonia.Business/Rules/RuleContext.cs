namespace Nerosoft.Euonia.Business;

public class RuleContext : IRuleContext
{
    private readonly Action<IRuleContext> _completeAction;
    private readonly List<RuleResult> _results = new();

    internal RuleContext(Action<IRuleContext> completeAction)
    {
        _completeAction = completeAction;
    }

    public IRuleBase Rule { get; internal set; }

    public object Target { get; internal set; }

    public string PropertyName { get; internal set; }

    public IReadOnlyList<RuleResult> Results => _results;

    public void AddErrorResult(string description)
    {
        _results.Add(new RuleResult(Rule.Name, description, RuleSeverity.Error));
    }

    public void AddWarningResult(string description)
    {
        _results.Add(new RuleResult(Rule.Name, description, RuleSeverity.Warning));
    }

    public void AddInformationResult(string description)
    {
        _results.Add(new RuleResult(Rule.Name, description, RuleSeverity.Information));
    }

    public void AddSuccessResult()
    {
        _results.Add(new RuleResult(Rule.Name) { Severity = RuleSeverity.Success });
    }

    public void Complete()
    {
        if (Results.Count == 0)
        {
            _results.Add(new RuleResult(Rule.Name));
        }

        _completeAction?.Invoke(this);
    }
}