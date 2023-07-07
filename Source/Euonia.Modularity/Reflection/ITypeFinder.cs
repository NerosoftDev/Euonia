namespace Nerosoft.Euonia.Modularity;

/// <summary>
/// This interface defines a contract for a type finder that retrieves an read-only list of types.
/// </summary>
public interface ITypeFinder
{
    /// <summary>
    /// Gets the list of types.
    /// </summary>
    IReadOnlyList<Type> Types { get; }
}