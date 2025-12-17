namespace Nerosoft.Euonia.Bus;

/// <summary>
/// Represents the message can be dispatched to local bus.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class LocalMessageAttribute : Attribute
{
}