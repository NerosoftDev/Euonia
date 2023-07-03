namespace Nerosoft.Euonia.Domain;

public interface INamedMessage : IMessage
{
    /// <summary>
    /// Gets the message name.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Gets the message data.
    /// </summary>
    object Data { get; }
}