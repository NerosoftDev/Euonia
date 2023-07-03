namespace Nerosoft.Euonia.Domain;

public class NamedCommand : Command, INamedMessage
{
    /// <summary>
    /// Initialize a new instance of <see cref="NamedCommand"/>.
    /// </summary>
    /// <param name="name"></param>
    public NamedCommand(string name)
    {
        Name = name;
    }

    /// <summary>
    /// Initialize a new instance of <see cref="NamedCommand"/>.
    /// </summary>
    /// <param name="name">The event name.</param>
    /// <param name="data">The event data.</param>
    public NamedCommand(string name, object data)
        : this(name)
    {
        Data = data;
    }

    /// <summary>
    /// Gets the event name.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the event data.
    /// </summary>
    public object Data { get; }
}
