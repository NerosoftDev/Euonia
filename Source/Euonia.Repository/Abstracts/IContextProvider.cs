namespace Nerosoft.Euonia.Repository;

/// <summary>
/// The interface used to create a <see cref="IRepositoryContext"/> instance.
/// </summary>
public interface IContextProvider
{
    /// <summary>
    /// Gets the repository context instance.
    /// </summary>
    /// <typeparam name="TContext"></typeparam>
    /// <returns></returns>
    TContext GetContext<TContext>()
        where TContext : class, IRepositoryContext;

    /// <summary>
    /// Sets the context factory.
    /// </summary>
    /// <param name="factory"></param>
    /// <typeparam name="TContext"></typeparam>
    void SetFactory<TContext>(Func<TContext> factory)
        where TContext : class, IRepositoryContext;
}