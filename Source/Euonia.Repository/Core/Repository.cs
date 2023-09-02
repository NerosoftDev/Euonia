using System.Linq.Expressions;
using Nerosoft.Euonia.Collections;
using Nerosoft.Euonia.Domain;
using Nerosoft.Euonia.Linq;

namespace Nerosoft.Euonia.Repository;

/// <summary>
/// The abstract implement of <see cref="IRepository{TEntity, TKey}"/>.
/// </summary>
/// <typeparam name="TEntity">The type of the entity.</typeparam>
/// <typeparam name="TKey">The type of the entity key.</typeparam>
/// <typeparam name="TContext"></typeparam>
public abstract class Repository<TContext, TEntity, TKey> : DisposableObject, IRepository<TContext, TEntity, TKey>
    where TKey : IEquatable<TKey>
    where TEntity : class, IEntity<TKey>
    where TContext : class, IRepositoryContext
{
    private readonly IContextProvider _provider;

    /// <summary>
    /// Initialize a new instance of <see cref="Repository{TContext, TEntity, TKey}"/> with context.
    /// </summary>
    /// <param name="provider"></param>
    protected Repository(IContextProvider provider)
    {
        _provider = provider;
    }

    /// <summary>
    /// Gets the data persistent context.
    /// </summary>
    public TContext Context => _provider.GetContext<TContext>();

    /// <inheritdoc />
    public List<Func<IQueryable<TEntity>, IQueryable<TEntity>>> Actions { get; } = new();

    /// <summary>
    /// Gets all entities.
    /// </summary>
    /// <returns></returns>
    public abstract IQueryable<TEntity> Queryable();

    /// <inherited/>
    public abstract Task<TEntity> GetAsync(TKey key, CancellationToken cancellationToken = default);

    /// <inherited/>
    public abstract Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

    /// <inherited/>
    public abstract IQueryable<TEntity> Fetch(Expression<Func<TEntity, bool>> predicate);

    /// <inherited/>
    public abstract IQueryable<TEntity> Fetch(Expression<Func<TEntity, bool>> predicate, Action<Orderable<TEntity>> order);

    /// <inherited/>
    public abstract IQueryable<TEntity> Fetch(Expression<Func<TEntity, bool>> predicate, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> order);

    /// <inheritdoc />
    public abstract Task<PageableCollection<TEntity>> FetchAsync(Expression<Func<TEntity, bool>> predicate, Action<Orderable<TEntity>> order, int? page, int? size, CancellationToken cancellationToken = default);

    /// <inheritdoc />
    public abstract Task<PageableCollection<TEntity>> FetchAsync(Expression<Func<TEntity, bool>> predicate, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> order, int? page, int? size, CancellationToken cancellationToken = default);

    /// <inheritdoc />
    public abstract Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

    /// <inheritdoc />
    public abstract Task<long> LongCountAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

    /// <inherited/>
    public abstract Task<TEntity> InsertAsync(TEntity entity, bool autoSave = true, CancellationToken cancellationToken = default);

    /// <inherited/>
    public abstract Task InsertAsync(IEnumerable<TEntity> entities, bool autoSave = true, CancellationToken cancellationToken = default);

    /// <inherited/>
    public abstract Task UpdateAsync(TEntity entity, bool autoSave = true, CancellationToken cancellationToken = default);

    /// <inherited/>
    public abstract Task UpdateAsync(IEnumerable<TEntity> entities, bool autoSave = true, CancellationToken cancellationToken = default);

    /// <inherited/>
    public abstract Task DeleteAsync(TEntity entity, bool autoSave = true, CancellationToken cancellationToken = default);

    /// <inherited/>
    public abstract Task DeleteAsync(IEnumerable<TEntity> entities, bool autoSave = true, CancellationToken cancellationToken = default);

    /// <summary>
    /// Commit changes asynchronously.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns></returns>
    public virtual async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await Context.SaveChangesAsync(cancellationToken);
    }
}