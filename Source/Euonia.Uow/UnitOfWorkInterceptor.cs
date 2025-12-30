using Castle.DynamicProxy;

namespace Nerosoft.Euonia.Uow;

/// <inheritdoc />
public class UnitOfWorkInterceptor : IInterceptor
{
    private readonly IUnitOfWorkManager _manager;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="manager"></param>
    public UnitOfWorkInterceptor(IUnitOfWorkManager manager)
    {
        _manager = manager;
    }

    /// <inheritdoc />
    public void Intercept(IInvocation invocation)
    {
        using (var uow = _manager.Begin())
        {
            invocation.Proceed();
            uow.CompleteAsync().Wait();
        }
    }
}