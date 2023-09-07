namespace Nerosoft.Euonia.Modularity;

/// <summary>
/// The exposed service types explorer.
/// </summary>
public static class ExposedServiceExplorer
{
    private static readonly ExposeServicesAttribute _defaultExposeServicesAttribute =
        new()
        {
            IncludeDefaults = true,
            IncludeSelf = true
        };

    /// <summary>
    /// Get the exposed service types of specified type.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static List<Type> GetExposedServices(Type type)
    {
        return type
            .GetCustomAttributes(true)
            .OfType<IExposedServiceTypesProvider>()
            .DefaultIfEmpty(_defaultExposeServicesAttribute)
            .SelectMany(p => p.GetExposedServiceTypes(type))
            .Distinct()
            .ToList();
    }
}
