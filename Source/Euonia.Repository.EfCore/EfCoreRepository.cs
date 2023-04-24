using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Nerosoft.Euonia.Collections;
using Nerosoft.Euonia.Domain;
using Nerosoft.Euonia.Linq;

namespace Nerosoft.Euonia.Repository.EfCore;

/// <inheritdoc />
public class EfCoreRepository<TContext, TEntity, TKey> : Repository<TContext, TEntity, TKey>
    where TKey : IEquatable<TKey>
    where TEntity : class, IEntity<TKey>
    where TContext : DbContext, IRepositoryContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EfCoreRepository{TContext, TEntity, TKey}"/> class.
    /// </summary>
    /// <param name="provider">The repository context.</param>
    public EfCoreRepository(IContextProvider provider)
        : base(provider)
    {
    }

    /// <inheritdoc />
    protected override void Dispose(bool disposing)
    {
        //Context?.Dispose();
    }

    /// <inheritdoc />
    public override IQueryable<TEntity> Queryable()
    {
        var query = Context.Set<TEntity>().AsQueryable();
        if (Actions.Count > 0)
        {
            query = Actions.Aggregate(query, (current, action) => action(current));
        }

        return query;
    }

    /// <inheritdoc />
    public override async Task<TEntity> GetAsync(TKey key, CancellationToken cancellationToken = default)
    {
        return await Context.FindAsync<TEntity>(key);
    }

    /// <inheritdoc />
    public override async Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await Context.Set<TEntity>().FirstOrDefaultAsync(predicate, cancellationToken);
    }

    /// <inheritdoc />
    public override IQueryable<TEntity> Fetch(Expression<Func<TEntity, bool>> predicate)
    {
        return Queryable().Where(predicate);
    }

    /// <inheritdoc />
    public override IQueryable<TEntity> Fetch(Expression<Func<TEntity, bool>> predicate, Action<Orderable<TEntity>> order)
    {
        var orderable = new Orderable<TEntity>(Fetch(predicate));
        order(orderable);
        return orderable.Queryable;
    }

    /// <inheritdoc />
    public override IQueryable<TEntity> Fetch(Expression<Func<TEntity, bool>> predicate, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> order)
    {
        var query = Queryable().Where(predicate);
        query = order(query);

        return query;
    }

    /// <inheritdoc />
    public override async Task<PageableCollection<TEntity>> FetchAsync(Expression<Func<TEntity, bool>> predicate, Action<Orderable<TEntity>> order, int? page, int? size, CancellationToken cancellationToken = default)
    {
        var pageIndex = page ?? 1;
        var pageSize = size ?? int.MaxValue;

        var handler = new QueryHandler<TEntity>(Queryable());

        handler.AddCriteria(predicate);
        var count = handler.GetCount();

        handler.SetPage(pageIndex).SetSize(pageSize);

        handler.SetCollator(order);

        var list = await handler.QueryAsync(async query => await query.ToListAsync(cancellationToken));

        return new PageableCollection<TEntity>(list) { TotalCount = count, PageNumber = pageIndex, PageSize = pageSize };
    }

    /// <inheritdoc />
    public override async Task<PageableCollection<TEntity>> FetchAsync(Expression<Func<TEntity, bool>> predicate, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> order, int? page, int? size, CancellationToken cancellationToken = default)
    {
        var pageIndex = page ?? 1;
        var pageSize = size ?? int.MaxValue;

        var handler = new QueryHandler<TEntity>(Queryable());

        handler.AddCriteria(predicate);
        var count = await handler.GetCountAsync(async query => await query.CountAsync(cancellationToken));

        handler.SetPage(pageIndex).SetSize(pageSize);

        handler.SetCollator(order);

        var list = await handler.QueryAsync(async query => await query.ToListAsync(cancellationToken));

        return new PageableCollection<TEntity>(list) { TotalCount = count, PageNumber = pageIndex, PageSize = pageSize };
    }

    /// <inheritdoc />
    public override async Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await Context.Set<TEntity>().CountAsync(predicate, cancellationToken);
    }

    /// <inheritdoc />
    public override async Task<long> LongCountAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await Context.Set<TEntity>().LongCountAsync(predicate, cancellationToken);
    }

    /// <inheritdoc />
    public override async Task<TEntity> InsertAsync(TEntity entity, bool autoSave = true, CancellationToken cancellationToken = default)
    {
        var entry = await Context.AddAsync(entity, cancellationToken);
        if (autoSave)
        {
            await SaveChangesAsync(cancellationToken);
        }

        return entry.Entity;
    }

    /// <inheritdoc />
    public override async Task InsertAsync(IEnumerable<TEntity> entities, bool autoSave = true, CancellationToken cancellationToken = default)
    {
        await Context.AddRangeAsync(entities, cancellationToken);
        if (autoSave)
        {
            await SaveChangesAsync(cancellationToken);
        }
    }

    /// <inheritdoc />
    public override async Task UpdateAsync(TEntity entity, bool autoSave = true, CancellationToken cancellationToken = default)
    {
        var _ = Context.Update(entity);
        if (autoSave)
        {
            await SaveChangesAsync(cancellationToken);
        }
    }

    /// <inheritdoc />
    public override async Task UpdateAsync(IEnumerable<TEntity> entities, bool autoSave = true, CancellationToken cancellationToken = default)
    {
        Context.UpdateRange(entities);
        if (autoSave)
        {
            await SaveChangesAsync(cancellationToken);
        }
    }

    /// <inheritdoc />
    public override async Task DeleteAsync(TEntity entity, bool autoSave = true, CancellationToken cancellationToken = default)
    {
        var _ = Context.Remove(entity);
        if (autoSave)
        {
            await SaveChangesAsync(cancellationToken);
        }
    }

    /// <inheritdoc />
    public override async Task DeleteAsync(IEnumerable<TEntity> entities, bool autoSave = true, CancellationToken cancellationToken = default)
    {
        Context.RemoveRange(entities);
        if (autoSave)
        {
            await SaveChangesAsync(cancellationToken);
        }
    }
}