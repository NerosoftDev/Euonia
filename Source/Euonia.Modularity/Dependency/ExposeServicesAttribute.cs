using System.Reflection;

namespace Nerosoft.Euonia.Modularity;

/// <summary>
/// Exposes all public services of the class.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class ExposeServicesAttribute : Attribute, IExposedServiceTypesProvider
{
    /// <summary>
    /// The list of exposed service types.
    /// </summary>
    public Type[] ServiceTypes { get; }

    /// <summary>
    /// Gets or sets a value indicating whether include defaults.
    /// </summary>
    public bool IncludeDefaults { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether include self.
    /// </summary>
    public bool IncludeSelf { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ExposeServicesAttribute"/> class.
    /// </summary>
    /// <param name="serviceTypes"></param>
    public ExposeServicesAttribute(params Type[] serviceTypes)
    {
        ServiceTypes = serviceTypes ?? Array.Empty<Type>();
    }

    /// <inheritdoc />
    public IEnumerable<Type> GetExposedServiceTypes(Type targetType)
    {
        var serviceList = ServiceTypes.ToList();

        if (IncludeDefaults)
        {
            foreach (var type in GetDefaultServices(targetType))
            {
                serviceList.AddIfNotContains(type);
            }

            if (IncludeSelf)
            {
                serviceList.AddIfNotContains(targetType);
            }
        }
        else if (IncludeSelf)
        {
            serviceList.AddIfNotContains(targetType);
        }

        return serviceList.ToArray();
    }

    private static List<Type> GetDefaultServices(Type type)
    {
        var serviceTypes = new List<Type>();

        foreach (var interfaceType in type.GetTypeInfo().GetInterfaces())
        {
            var interfaceName = interfaceType.Name;

            if (interfaceName.StartsWith("I"))
            {
                interfaceName = interfaceName.Right(interfaceName.Length - 1);
            }

            if (type.Name.EndsWith(interfaceName))
            {
                serviceTypes.Add(interfaceType);
            }
        }

        return serviceTypes;
    }
}