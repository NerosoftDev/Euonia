using System.Linq.Expressions;

namespace Nerosoft.Euonia.Linq;

public class QueryHandler<TEntity>
{
    private readonly IList<Expression<Func<TEntity, bool>>> _predicates;

    private IQueryable<TEntity> _query;

    private int _page = 1;
    private int _size = int.MaxValue;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="query"></param>
    public QueryHandler(IQueryable<TEntity> query)
    {
        _predicates = new List<Expression<Func<TEntity, bool>>>();
        _query = query;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public QueryHandler<TEntity> AddCriteria(Expression<Func<TEntity, bool>> predicate)
    {
        _predicates.Add(predicate);
        return this;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public IList<TEntity> Query()
    {
        var predication = _predicates.Aggregate<Expression<Func<TEntity, bool>>, Expression<Func<TEntity, bool>>>(null, (current, predicate) => (current == null ? predicate : current.And(predicate)));

        _query = _query.Where(predication);

        _query = _query.Skip((_page - 1) * _size).Take(_size);

        return _query.ToList();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public int GetCount()
    {
        var predicate = _predicates.Compose();

        // foreach (var criterion in _predicates)
        // {
        //     _query = _query.Where(criterion);
        // }

        _query = _query.Where(predicate);

        return _query.Count();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="action"></param>
    /// <returns></returns>
    public async Task<IList<TEntity>> QueryAsync(Func<IQueryable<TEntity>, Task<IList<TEntity>>> action)
    {
        var predication = _predicates.Aggregate<Expression<Func<TEntity, bool>>, Expression<Func<TEntity, bool>>>(null, (current, predicate) => (current == null ? predicate : current.And(predicate)));

        _query = _query.Where(predication);

        _query = _query.Skip((_page - 1) * _size).Take(_size);
        return await action(_query);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public async Task<int> GetCountAsync(Func<IQueryable<TEntity>, Task<int>> action)
    {
        var predicate = _predicates.Compose();
        _query = _query.Where(predicate);
        return await action(_query);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public QueryHandler<TEntity> SetPage(int index)
    {
        _page = index;
        return this;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="size"></param>
    /// <returns></returns>
    public QueryHandler<TEntity> SetSize(int size)
    {
        _size = size;
        return this;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="keySelector"></param>
    /// <returns></returns>
    public QueryHandler<TEntity> OrderByAscending<TResult>(Expression<Func<TEntity, TResult>> keySelector)
    {
        _query = _query.OrderBy(keySelector);

        return this;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="keySelector"></param>
    /// <returns></returns>
    public QueryHandler<TEntity> OrderByDescending<TResult>(Expression<Func<TEntity, TResult>> keySelector)
    {
        _query = _query.OrderByDescending(keySelector);
        return this;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="order"></param>
    /// <returns></returns>
    public QueryHandler<TEntity> SetCollator(Action<Orderable<TEntity>> order)
    {
        var orderable = new Orderable<TEntity>(_query);
        order(orderable);
        _query = orderable.Queryable;
        return this;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="order"></param>
    /// <returns></returns>
    public QueryHandler<TEntity> SetCollator(Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> order)
    {
        var orderable = new Orderable<TEntity>(_query);
        _query = order(orderable.Queryable);
        return this;
    }
}
