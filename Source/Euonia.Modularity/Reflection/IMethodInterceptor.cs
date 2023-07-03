namespace Nerosoft.Euonia.Modularity;

public interface IMethodInterceptor
{
    Task InterceptAsync(IMethodInvocation invocation);
}