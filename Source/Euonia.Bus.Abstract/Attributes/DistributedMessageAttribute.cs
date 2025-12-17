namespace Nerosoft.Euonia.Bus;

/// <summary>
/// Represents the message can be dispatched to distributed bus.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class DistributedMessageAttribute : Attribute
{
}