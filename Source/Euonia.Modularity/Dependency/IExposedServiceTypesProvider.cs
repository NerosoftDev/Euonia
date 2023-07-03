namespace Nerosoft.Euonia.Modularity;

public interface IExposedServiceTypesProvider
{
    IEnumerable<Type> GetExposedServiceTypes(Type targetType);
}
