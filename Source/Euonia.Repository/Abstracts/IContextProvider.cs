namespace Nerosoft.Euonia.Repository;

public interface IContextProvider
{
    TContext GetContext<TContext>()
        where TContext : class, IRepositoryContext;

    void SetFactory<TContext>(Func<TContext> factory)
        where TContext : class, IRepositoryContext;
}