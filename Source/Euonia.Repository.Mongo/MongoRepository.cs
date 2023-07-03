using System.Linq.Expressions;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Nerosoft.Euonia.Collections;
using Nerosoft.Euonia.Domain;
using Nerosoft.Euonia.Linq;

namespace Nerosoft.Euonia.Repository.Mongo;

public class MongoRepository<TContext, TEntity, TKey> : Repository<TContext, TEntity, TKey>
    where TKey : IEquatable<TKey>
    where TEntity : class, IEntity<TKey>
    where TContext : MongoDbContext, IRepositoryContext
{
    public MongoRepository(IContextProvider provider)
        : base(provider)
    {
    }

    protected override void Dispose(bool disposing)
    {
        // ignore.
    }

    public override IQueryable<TEntity> Queryable()
    {
        IQueryable<TEntity> query = Context.Collection<TEntity>().AsQueryable();
        if (Actions.Count > 0)
        {
            query = Actions.Aggregate(query, (current, action) => action(current));
        }

        return query;
    }

    public override async Task<TEntity> GetAsync(TKey key, CancellationToken cancellationToken = default)
    {
        var id = MongoDB.Bson.ObjectId.Parse(key.ToString());
        return await Context.FindAsync<TEntity>(id, cancellationToken);
    }

    public override async Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        var options = new FindOptions<TEntity> { Limit = 1 };
        var query = await Context.FindAsync(predicate, options, cancellationToken);
        return await query.FirstOrDefaultAsync(cancellationToken);
    }

    public override IQueryable<TEntity> Fetch(Expression<Func<TEntity, bool>> predicate)
    {
        return System.Linq.Queryable.Where(Context.Collection<TEntity>().AsQueryable(), predicate);
    }

    public override IQueryable<TEntity> Fetch(Expression<Func<TEntity, bool>> predicate, Action<Orderable<TEntity>> order)
    {
        var orderable = new Orderable<TEntity>(Fetch(predicate));
        order(orderable);
        return orderable.Queryable;
    }

    public override IQueryable<TEntity> Fetch(Expression<Func<TEntity, bool>> predicate, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> order)
    {
        var query = Queryable().Where(predicate);
        query = order(query);
        return query;
    }

    public override async Task<PageableCollection<TEntity>> FetchAsync(Expression<Func<TEntity, bool>> predicate, Action<Orderable<TEntity>> order, int? page, int? size, CancellationToken cancellationToken = default)
    {
        var pageIndex = page ?? 1;
        var pageSize = size ?? int.MaxValue;

        var handler = new QueryHandler<TEntity>(Queryable());

        handler.AddCriteria(predicate);
        var count = handler.GetCount();

        handler.SetPage(pageIndex).SetSize(pageSize);

        handler.SetCollator(order);

        var list = await handler.QueryAsync(async query => await ((IMongoQueryable<TEntity>)query).ToListAsync(cancellationToken));

        return new PageableCollection<TEntity>(list) { TotalCount = count, PageNumber = pageIndex, PageSize = pageSize };
    }

    public override async Task<PageableCollection<TEntity>> FetchAsync(Expression<Func<TEntity, bool>> predicate, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> order, int? page, int? size, CancellationToken cancellationToken = default)
    {
        var pageIndex = page ?? 1;
        var pageSize = size ?? int.MaxValue;

        var handler = new QueryHandler<TEntity>(Queryable());

        handler.AddCriteria(predicate);
        var count = await handler.GetCountAsync(async query => await ((IMongoQueryable<TEntity>)query).CountAsync(cancellationToken));

        handler.SetPage(pageIndex).SetSize(pageSize);

        handler.SetCollator(order);

        var list = await handler.QueryAsync(async query => await ((IMongoQueryable<TEntity>)query).ToListAsync(cancellationToken));

        return new PageableCollection<TEntity>(list) { TotalCount = count, PageNumber = pageIndex, PageSize = pageSize };
    }

    public override async Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        var result = await Context.Collection<TEntity>().CountDocumentsAsync(predicate, null, cancellationToken);
        return (int)result;
    }

    public override async Task<long> LongCountAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        var result = await Context.Collection<TEntity>().CountDocumentsAsync(predicate, null, cancellationToken);
        return result;
    }

    public override async Task<TEntity> InsertAsync(TEntity entity, bool autoSave = true, CancellationToken cancellationToken = default)
    {
        var document = await Context.InsertAsync(entity, cancellationToken);
        //var id = document["_id"];
        var result = BsonSerializer.Deserialize<TEntity>(document);
        return result;
    }

    public override async Task InsertAsync(IEnumerable<TEntity> entities, bool autoSave = true, CancellationToken cancellationToken = default)
    {
        await Context.InsertAsync(entities, cancellationToken);
    }

    public override async Task UpdateAsync(TEntity entity, bool autoSave = true, CancellationToken cancellationToken = default)
    {
        await Context.UpdateAsync(MongoDB.Bson.ObjectId.Parse(entity.Id.ToString()), entity, cancellationToken);
    }

    public override async Task UpdateAsync(IEnumerable<TEntity> entities, bool autoSave = true, CancellationToken cancellationToken = default)
    {
        await Context.UpdateAsync(entities, t => t.Id, cancellationToken);
    }

    public override async Task DeleteAsync(TEntity entity, bool autoSave = true, CancellationToken cancellationToken = default)
    {
        await Context.DeleteAsync<TEntity>(MongoDB.Bson.ObjectId.Parse(entity.Id.ToString()), cancellationToken);
    }

    /// <inheritdoc />
    public override async Task DeleteAsync(IEnumerable<TEntity> entities, bool autoSave = true, CancellationToken cancellationToken = default)
    {
        var keys = entities.Select(t => t.Id);
        await Context.DeleteAsync<TEntity>(t => keys.Contains(t.Id), cancellationToken);
    }
}