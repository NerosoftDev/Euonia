using System.Linq.Expressions;
using Nerosoft.Euonia.Collections;
using Nerosoft.Euonia.Domain;
using Nerosoft.Euonia.Linq;

namespace Nerosoft.Euonia.Repository;

/// <summary>
/// 
/// </summary>
/// <typeparam name="TEntity"></typeparam>
public interface IRepository<TEntity> : IDisposable
    where TEntity : class
{
    List<Func<IQueryable<TEntity>, IQueryable<TEntity>>> Actions { get; }

    /// <summary>
    /// Queryable this instance.
    /// </summary>
    /// <returns>The all.</returns>
    IQueryable<TEntity> Queryable();

    /// <summary>
    /// Gets an entity with the given primary key value asynchronously.
    /// </summary>
    /// <param name="predicate"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets entities matches the specified predicate.
    /// </summary>
    /// <returns>The fetch.</returns>
    /// <param name="predicate">Predicate.</param>
    IQueryable<TEntity> Fetch(Expression<Func<TEntity, bool>> predicate);

    /// <summary>
    /// Gets entities matches the specified predicate.
    /// </summary>
    /// <returns>The fetch.</returns>
    /// <param name="predicate">Predicate.</param>
    /// <param name="order">Order.</param>
    IQueryable<TEntity> Fetch(Expression<Func<TEntity, bool>> predicate, Action<Orderable<TEntity>> order);

    /// <summary>
    /// Gets entities matches the specified predicate.
    /// </summary>
    /// <returns>The fetch.</returns>
    /// <param name="predicate">Predicate.</param>
    /// <param name="order">Order.</param>
    IQueryable<TEntity> Fetch(Expression<Func<TEntity, bool>> predicate, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> order);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="predicate"></param>
    /// <param name="order"></param>
    /// <param name="page"></param>
    /// <param name="size"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<PageableCollection<TEntity>> FetchAsync(Expression<Func<TEntity, bool>> predicate, Action<Orderable<TEntity>> order, int? page, int? size, CancellationToken cancellationToken = default);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="predicate"></param>
    /// <param name="order"></param>
    /// <param name="page"></param>
    /// <param name="size"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<PageableCollection<TEntity>> FetchAsync(Expression<Func<TEntity, bool>> predicate, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> order, int? page, int? size, CancellationToken cancellationToken = default);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="predicate"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="predicate"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<long> LongCountAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Insert a new entity asynchronously.
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="autoSave"></param>
    /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
    /// <returns></returns>
    Task<TEntity> InsertAsync(TEntity entity, bool autoSave = true, CancellationToken cancellationToken = default);

    /// <summary>
    /// Insert multiple new entities asynchronously.
    /// </summary>
    /// <param name="entities"></param>
    /// <param name="autoSave"></param>
    /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
    /// <returns></returns>
    Task InsertAsync(IEnumerable<TEntity> entities, bool autoSave = true, CancellationToken cancellationToken = default);

    /// <summary>
    /// Update an exists entity asynchronously.
    /// </summary>
    /// <param name="entity">The entity.</param>
    /// <param name="autoSave">A <see cref="bool"/> value indicate that whether the <see cref="SaveChangesAsync" /> will called automatically.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
    /// <returns></returns>
    Task UpdateAsync(TEntity entity, bool autoSave = true, CancellationToken cancellationToken = default);

    /// <summary>
    /// Update multiple exists entities asynchronously.
    /// </summary>
    /// <param name="entities"></param>
    /// <param name="autoSave">A <see cref="bool"/> value indicate that whether the <see cref="SaveChangesAsync" /> will called automatically.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
    /// <returns></returns>
    Task UpdateAsync(IEnumerable<TEntity> entities, bool autoSave = true, CancellationToken cancellationToken = default);

    /// <summary>
    /// DeleteAsync the specified entity.
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="autoSave"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task DeleteAsync(TEntity entity, bool autoSave = true, CancellationToken cancellationToken = default);

    /// <summary>
    /// DeleteAsync the specified entities.
    /// </summary>
    /// <param name="entities"></param>
    /// <param name="autoSave"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task DeleteAsync(IEnumerable<TEntity> entities, bool autoSave = true, CancellationToken cancellationToken = default);

    /// <summary>
    /// Saves the changes async.
    /// </summary>
    /// <returns>The changes async.</returns>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// 
/// </summary>
/// <typeparam name="TEntity"></typeparam>
/// <typeparam name="TKey"></typeparam>
public interface IRepository<TEntity, in TKey> : IRepository<TEntity>
    where TKey : IEquatable<TKey>
    where TEntity : class, IEntity<TKey>
{
    /// <summary>
    /// Gets an entity with the given primary key value asynchronously.
    /// </summary>
    /// <param name="key">The key of entity.</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<TEntity> GetAsync(TKey key, CancellationToken cancellationToken = default);
}

/// <summary>
/// The interface of repository.
/// </summary>
/// <typeparam name="TEntity">The type if entity.</typeparam>
/// <typeparam name="TKey">The type of entity identifier.</typeparam>
/// <typeparam name="TContext">The type of context.</typeparam>
public interface IRepository<out TContext, TEntity, in TKey> : IRepository<TEntity, TKey>
    where TKey : IEquatable<TKey>
    where TEntity : class, IEntity<TKey>
    where TContext : class, IRepositoryContext
{
    /// <summary>
    /// Gets the context.
    /// </summary>
    /// <value>The context.</value>
    TContext Context { get; }
}