namespace Nerosoft.Euonia.Repository;

/// <summary>
/// The interface used to create a <see cref="IRepositoryContext"/> instance.
/// </summary>
public interface IContextFactory : ITransientDependency
{
    /// <summary>
    /// Gets the repository context instance.
    /// </summary>
    /// <typeparam name="TContext"></typeparam>
    /// <returns></returns>
    TContext GetContext<TContext>()
        where TContext : class, IRepositoryContext;

    /// <summary>
    /// Gets the order of the context factory.
    /// </summary>
    int Order { get; }
}