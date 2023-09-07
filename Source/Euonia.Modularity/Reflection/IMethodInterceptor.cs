namespace Nerosoft.Euonia.Modularity;

/// <summary>
/// The contract interface of the method interceptor.
/// </summary>
public interface IMethodInterceptor
{
	/// <summary>
	/// Intercept the method invocation.
	/// </summary>
	/// <param name="invocation"></param>
	/// <returns></returns>
    Task InterceptAsync(IMethodInvocation invocation);
}