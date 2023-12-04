namespace Nerosoft.Euonia.Bus;

/// <summary>
/// Represents the decorator for the request.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public class RequestAttribute : Attribute
{
}