namespace Nerosoft.Euonia.Bus;

/// <summary>
/// Represents the attributed method would handle an event.
/// </summary>
[AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
public class EventSubscribeAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EventSubscribeAttribute"/> class.
    /// </summary>
    /// <param name="name"></param>
    public EventSubscribeAttribute(string name)
    {
        Name = name;
    }

    /// <summary>
    /// Gets the name of the event.
    /// </summary>
    public string Name { get; }
}