namespace Nerosoft.Euonia.Repository;

/// <summary>
/// Interface IUnitOfWork
/// Implements the <see cref="IDisposable" />
/// </summary>
/// <seealso cref="IDisposable" />
public interface IUnitOfWork : IDisposable
{
    /// <summary>
    /// 
    /// </summary>
    event EventHandler<DisposedEventArgs> Disposed;

    /// <summary>
    /// 
    /// </summary>
    event EventHandler<UnitOfWorkEventArgs> Completed;

    /// <summary>
    /// 
    /// </summary>
    event EventHandler<UnitOfWorkFailedEventArgs> Failed;

    /// <summary>
    /// Gets the instance identifier.
    /// </summary>
    Guid Id { get; }

    /// <summary>
    /// Gets the items.
    /// </summary>
    Dictionary<string, object> Items { get; }

    /// <summary>
    /// Gets the contexts.
    /// </summary>
    IReadOnlyDictionary<Type, IRepositoryContext> Contexts { get; }

    /// <summary>
    /// Gets the service provider.
    /// </summary>
    IServiceProvider ServiceProvider { get; }

    /// <summary>
    /// 
    /// </summary>
    IUnitOfWork Outer { get; }

    /// <summary>
    /// 
    /// </summary>
    bool IsReserved { get; }

    /// <summary>
    /// 
    /// </summary>
    string ReservationName { get; }

    /// <summary>
    /// 
    /// </summary>
    bool IsDisposed { get; }

    /// <summary>
    /// Gets a value indicating whether the operation is completed.
    /// </summary>
    bool IsCompleted { get; }

    /// <summary>
    /// Commits changes asynchronously.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>Task&lt;System.Boolean&gt;.</returns>
    Task SaveChangesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Rollbacks changes.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task RollbackAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Commits changes asynchronously.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task CommitAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Register handler for Completed event.
    /// </summary>
    /// <param name="handler"></param>
    void OnCompleted(Func<Task> handler);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="outer"></param>
    void SetOuter(IUnitOfWork outer);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="options"></param>
    void Initialize(UnitOfWorkOptions options);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="reservationName"></param>
    void Reserve(string reservationName);

    /// <summary>
    /// Gets a instance of <typeparamref name="TService"/> in current scope of unit of work.
    /// </summary>
    /// <typeparam name="TService">The type of service object to get.</typeparam>
    /// <returns></returns>
    TService GetService<TService>() where TService : class;

    /// <summary>
    /// Gets instances of <typeparamref name="TService"/> in current scope of unit of work.
    /// </summary>
    /// <typeparam name="TService">The type of service object to get.</typeparam>
    /// <returns></returns>
    IEnumerable<TService> GetServices<TService>() where TService : class;

    /// <summary>
    /// Gets a instance of specified type in current scope of unit of work.
    /// </summary>
    /// <param name="serviceType">The type of service object to get.</param>
    /// <returns></returns>
    object GetService(Type serviceType);

    /// <summary>
    /// Gets instances of specified type in current scope of unit of work.
    /// </summary>
    /// <param name="serviceType">The type of service object to get.</param>
    /// <returns></returns>
    IEnumerable<object> GetServices(Type serviceType);

    /// <summary>
    /// Creates a new <see cref="IRepositoryContext"/> instance with specified connection string.
    /// </summary>
    /// <param name="connectionString"></param>
    /// <typeparam name="TContext"></typeparam>
    /// <returns></returns>
    TContext CreateContext<TContext>(string connectionString)
        where TContext : IRepositoryContext;

    /// <summary>
    /// Creates a new <see cref="IRepositoryContext"/> instance.
    /// </summary>
    /// <typeparam name="TContext"></typeparam>
    /// <returns></returns>
    TContext CreateContext<TContext>()
        where TContext : IRepositoryContext;
}