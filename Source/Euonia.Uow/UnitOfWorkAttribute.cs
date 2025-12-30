namespace Nerosoft.Euonia.Uow;

/// <summary>
/// The attribute used to mark a class or method to use UOW pattern.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Interface)]
public class UnitOfWorkAttribute : Attribute
{
}