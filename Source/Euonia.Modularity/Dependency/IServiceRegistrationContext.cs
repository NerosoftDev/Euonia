using Nerosoft.Euonia.Collections;

namespace Nerosoft.Euonia.Modularity;

/// <summary>
/// The service registration context interface.
/// </summary>
public interface IServiceRegistrationContext
{
    /// <summary>
    /// Gets the method interceptor types.
    /// </summary>
    ITypeList<IMethodInterceptor> Interceptors { get; }

    /// <summary>
    /// Gets the implementation type.
    /// </summary>
    Type ImplementationType { get; }
}