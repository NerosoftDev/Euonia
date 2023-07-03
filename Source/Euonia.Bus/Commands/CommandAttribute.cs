namespace Nerosoft.Euonia.Bus;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public abstract class CommandAttribute : Attribute
{
}

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class DistributedCommandAttribute : CommandAttribute
{
}

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class IntegratedCommandAttribute : CommandAttribute
{
}