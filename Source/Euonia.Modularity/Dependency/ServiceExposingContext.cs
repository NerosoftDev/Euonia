namespace Nerosoft.Euonia.Modularity;

public class ServiceExposingContext : IServiceExposingContext
{
    public Type ImplementationType { get; }

    public List<Type> ExposedTypes { get; }

    public ServiceExposingContext(Type implementationType, List<Type> exposedTypes)
    {
        ImplementationType = Check.EnsureNotNull(implementationType, nameof(implementationType));
        ExposedTypes = Check.EnsureNotNull(exposedTypes, nameof(exposedTypes));
    }
}
