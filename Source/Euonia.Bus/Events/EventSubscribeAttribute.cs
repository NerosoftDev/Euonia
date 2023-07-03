namespace Nerosoft.Euonia.Bus;

[AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
public class EventSubscribeAttribute : Attribute
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="name"></param>
    public EventSubscribeAttribute(string name)
    {
        Name = name;
    }

    /// <summary>
    /// 
    /// </summary>
    public string Name { get; }
}