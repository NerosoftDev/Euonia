namespace Nerosoft.Euonia.Modularity;

public static class ExposedServiceExplorer
{
    private static readonly ExposeServicesAttribute _defaultExposeServicesAttribute =
        new()
        {
            IncludeDefaults = true,
            IncludeSelf = true
        };

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
