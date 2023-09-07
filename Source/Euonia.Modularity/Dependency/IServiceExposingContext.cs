namespace Nerosoft.Euonia.Modularity;

/// <summary>
/// The service exposing context.
/// </summary>
public interface IServiceExposingContext
{
    /// <summary>
    /// Gets the implementation type.
    /// </summary>
    Type ImplementationType { get; }

    /// <summary>
    /// Gets the exposed types of the implementation type.
    /// </summary>
    List<Type> ExposedTypes { get; }
}
