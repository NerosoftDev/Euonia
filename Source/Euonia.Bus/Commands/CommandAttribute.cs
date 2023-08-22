namespace Nerosoft.Euonia.Bus;

/// <summary>
/// Represents the attributed class is a command.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public abstract class CommandAttribute : Attribute
{
}

/// <summary>
/// Represents the attributed class is a distributed command.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class DistributedCommandAttribute : CommandAttribute
{
}

/// <summary>
/// Represents the attributed class is an integrated command.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class IntegratedCommandAttribute : CommandAttribute
{
}