namespace Nerosoft.Euonia.Modularity;

public interface IServiceExposingContext
{
    Type ImplementationType { get; }

    List<Type> ExposedTypes { get; }
}
