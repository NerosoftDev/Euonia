﻿namespace Nerosoft.Euonia.Business;

public interface IHasRuleCheck
{
    /// <summary>
    /// Gets a value indicate that whether the object is currently valid or not.
    /// </summary>
    /// <returns><c>True</c> if the object is currently valid, otherwise <c>False</c>.</returns>
    bool IsValid { get; }

    /// <summary>
    /// Indicates that a rule has completed.
    /// </summary>
    /// <param name="property"></param>
    void RuleCheckComplete(IPropertyInfo property);

    /// <summary>
    /// Indicates that a rule has completed.
    /// </summary>
    /// <param name="property"></param>
    void RuleCheckComplete(string property);

    /// <summary>
    /// Indicates that all rules have completed.
    /// </summary>
    void AllRulesComplete();

    void SuspendRuleChecking();

    void ResumeRuleChecking();

    /// <summary>
    /// Gets the broken rules for this object
    /// </summary>
    /// <returns></returns>
    BrokenRuleCollection GetBrokenRules();
}