using System.Linq.Expressions;

namespace Nerosoft.Euonia.Linq;

/// <summary>
/// Handles queries for the specified entity type.
/// </summary>
/// <typeparam name="TEntity"></typeparam>
public class QueryHandler<TEntity>
{
    private readonly List<Expression<Func<TEntity, bool>>> _predicates;

    private IQueryable<TEntity> _query;

    private int _page = 1;
    private int _size = int.MaxValue;

    /// <summary>
    /// Initializes a new instance of the <see cref="QueryHandler{TEntity}"/> class.
    /// </summary>
    /// <param name="query"></param>
    public QueryHandler(IQueryable<TEntity> query)
    {
        _predicates = new List<Expression<Func<TEntity, bool>>>();
        _query = query;
    }

    /// <summary>
    /// Adds a predicate to the query.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public QueryHandler<TEntity> AddCriteria(Expression<Func<TEntity, bool>> predicate)
    {
        _predicates.Add(predicate);
        return this;
    }

    /// <summary>
    /// Gets elements from the sequence.
    /// </summary>
    /// <returns></returns>
    public IList<TEntity> Query()
    {
	    var predication = _predicates.Compose();//.Aggregate<Expression<Func<TEntity, bool>>, Expression<Func<TEntity, bool>>>(null, (current, predicate) => (current == null ? predicate : current.And(predicate)));

        _query = _query.Where(predication);

        _query = _query.Skip((_page - 1) * _size).Take(_size);

        return _query.ToList();
    }

    /// <summary>
    /// Gets number of elements in the sequence.
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
    /// Gets elements from the sequence.
    /// </summary>
    /// <param name="action"></param>
    /// <returns></returns>
    public async Task<IList<TEntity>> QueryAsync(Func<IQueryable<TEntity>, Task<IList<TEntity>>> action)
    {
	    var predication = _predicates.Compose();//.Aggregate<Expression<Func<TEntity, bool>>, Expression<Func<TEntity, bool>>>(null, (current, predicate) => (current == null ? predicate : current.And(predicate)));

        _query = _query.Where(predication);

        _query = _query.Skip((_page - 1) * _size).Take(_size);
        return await action(_query);
    }

    /// <summary>
    /// Gets number of elements in the sequence.
    /// </summary>
    /// <returns></returns>
    public async Task<int> GetCountAsync(Func<IQueryable<TEntity>, Task<int>> action)
    {
        var predicate = _predicates.Compose();
        _query = _query.Where(predicate);
        return await action(_query);
    }

    /// <summary>
    /// Sets the non-zero based page number.
    /// </summary>
    /// <param name="page"></param>
    /// <returns></returns>
    public QueryHandler<TEntity> SetPage(int page)
    {
        _page = page;
        return this;
    }

    /// <summary>
    /// Sets the size of the page.
    /// </summary>
    /// <param name="size"></param>
    /// <returns></returns>
    public QueryHandler<TEntity> SetSize(int size)
    {
        _size = size;
        return this;
    }

    /// <summary>
    /// Sorts the elements of a sequence in ascending order according to a key.
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
    /// Sorts the query in descending order by using the specified key.
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
    /// Sets the collator.
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
    /// Sets the collator.
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
