namespace Nerosoft.Euonia.Repository;

/// <summary>
/// The <see cref="IContextFactory"/> implementation used to create a <see cref="IRepositoryContext"/> instance.
/// </summary>
public class UnitOfWorkContextFactory : IContextFactory
{
    private readonly IUnitOfWorkManager _manager;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="manager"></param>
    public UnitOfWorkContextFactory(IUnitOfWorkManager manager)
    {
        _manager = manager;
    }

    /// <inheritdoc />
    public TContext GetContext<TContext>()
        where TContext : class, IRepositoryContext
    {
        var unitOfWork = _manager.Current;
        if (unitOfWork == null)
        {
            return null;
        }

        var context = unitOfWork.Contexts.TryGetValue(typeof(TContext), out var ctx)
            ? ctx as TContext
            : null;

        context ??= unitOfWork.CreateContext<TContext>();
        return context;
    }

    /// <inheritdoc />
    public int Order => 1;
}