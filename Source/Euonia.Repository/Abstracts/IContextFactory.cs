namespace Nerosoft.Euonia.Repository;

public interface IContextFactory : ITransientDependency
{
    TContext GetContext<TContext>()
        where TContext : class, IRepositoryContext;

    int Order { get; }
}