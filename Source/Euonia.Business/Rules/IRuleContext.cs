namespace Nerosoft.Euonia.Business;

/// <summary>
/// Context information provided to a business rule when it is invoked.
/// </summary>
public interface IRuleContext
{
    /// <summary>
    /// Gets the rule object.
    /// </summary>
    IRuleBase Rule { get; }

    /// <summary>
    /// Gets a reference to the target business object.
    /// </summary>
    object Target { get; }

    /// <summary>
    /// Gets the rule check results.
    /// </summary>
    IReadOnlyList<RuleResult> Results { get; }

    void AddErrorResult(string description);

    void AddWarningResult(string description);

    void AddInformationResult(string description);

    void AddSuccessResult();

    void Complete();
}