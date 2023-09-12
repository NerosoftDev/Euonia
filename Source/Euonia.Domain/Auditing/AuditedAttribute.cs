namespace Nerosoft.Euonia.Domain;

/// <summary>
/// Represents that the decorated class should be audited.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property)]
public class AuditedAttribute : Attribute
{
}