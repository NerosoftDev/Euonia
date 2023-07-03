namespace Nerosoft.Euonia.Mapping;

/// <summary>
/// Base contract for adapter factory
/// </summary>
public interface ITypeAdapterFactory
{
    /// <summary>
    /// Create a type adapter
    /// </summary>
    /// <returns>The created <see cref="ITypeAdapter"/></returns>
    ITypeAdapter Create();
}