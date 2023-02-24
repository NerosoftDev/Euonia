using Nerosoft.Euonia.Collections;

namespace Nerosoft.Euonia.Modularity;

public interface IServiceRegistrationContext
{
    ITypeList<IMethodInterceptor> Interceptors { get; }

    Type ImplementationType { get; }
}