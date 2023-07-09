namespace Nerosoft.Euonia.Business;

/// <summary>
/// Represent the marked method would delete domain object data.
/// And the method could be invoked by <see cref="IObjectFactory"/>.
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class FactoryDeleteAttribute : FactoryMethodAttribute
{
}