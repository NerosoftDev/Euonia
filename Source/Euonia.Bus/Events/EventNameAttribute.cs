using System.Reflection;
using Nerosoft.Euonia.Domain;

namespace Nerosoft.Euonia.Bus;

/// <summary>
/// Represents the attributed event has a specified name.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class EventNameAttribute : Attribute
{
    /// <summary>
    /// Gets the event name.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Initialize a new instance of <see cref="EventNameAttribute"/>.
    /// </summary>
    /// <param name="name"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public EventNameAttribute(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentNullException(nameof(name));
        }

        Name = name;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TEvent"></typeparam>
    /// <returns></returns>
    public static string GetName<TEvent>()
        where TEvent : IEvent
    {
        return GetName(typeof(TEvent));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="eventType"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static string GetName(Type eventType)
    {
        if (eventType == null)
        {
            throw new ArgumentNullException(nameof(eventType));
        }

        return eventType.GetCustomAttribute<EventNameAttribute>()?.Name ?? eventType.Name;
    }
}