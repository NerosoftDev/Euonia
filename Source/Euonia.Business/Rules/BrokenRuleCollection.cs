using System.Collections.ObjectModel;

namespace Nerosoft.Euonia.Business;

public class BrokenRuleCollection : ObservableCollection<BrokenRule>
{
    private static readonly object _lockObject = new();

    public int ErrorCount { get; private set; }

    public int WarningCount { get; private set; }

    public int InformationCount { get; private set; }

    internal void ClearRules()
    {
        lock (_lockObject)
        {
            Clear();
            ErrorCount = WarningCount = InformationCount = 0;
        }
    }

    internal void ClearRules(IPropertyInfo property)
    {
        ClearRules(property?.Name);
    }

    private void ClearRules(string propertyName)
    {
        lock (_lockObject)
        {
            var count = Count;
            for (var index = 0; index < count;)
            {
                var rule = this[index];
                if (rule.Property != propertyName)
                {
                    index++;
                }
                else
                {
                    RemoveItem(index);
                    count--;
                }
            }
        }
    }

    internal void Add(IEnumerable<RuleResult> results, string propertyName)
    {
        lock (_lockObject)
        {
            var rules = new HashSet<string>();

            foreach (var result in results)
            {
                ClearRules(propertyName);
                if (result.Success)
                {
                    continue;
                }

                if (string.IsNullOrWhiteSpace(result.Description))
                {
                    throw new ArgumentNullException(nameof(result.Description), "Rule message is required.");
                }

                var rule = new BrokenRule()
                {
                    Description = result.Description,
                    Severity = result.Severity,
                    Property = propertyName
                };

                Add(rule);
            }
        }
    }

    private new void Add(BrokenRule item)
    {
        base.Add(item);
        CountOne(item.Severity, 1);
    }

    private new void RemoveItem(int i)
    {
        CountOne(this[i].Severity, -1);

        base.RemoveItem(i);
    }

    private void CountOne(RuleSeverity severity, int one)
    {
        switch (severity)
        {
            case RuleSeverity.Error:
                ErrorCount += one;
                break;
            case RuleSeverity.Warning:
                WarningCount += one;
                break;
            case RuleSeverity.Information:
                InformationCount += one;
                break;
            case RuleSeverity.Success:
            default:
                throw new Exception("unhandled severity=" + severity);
        }
    }
}