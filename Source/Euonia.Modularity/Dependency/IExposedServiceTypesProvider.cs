namespace Nerosoft.Euonia.Modularity;

/// <summary>
/// The contract of exposed service types provider.
/// </summary>
public interface IExposedServiceTypesProvider
{
    /// <summary>
    /// Gets the exposed service types of the given target type.
    /// </summary>
    /// <param name="targetType"></param>
    /// <returns></returns>
    IEnumerable<Type> GetExposedServiceTypes(Type targetType);
}
