using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;
using Nerosoft.Euonia.Collections;
using Nerosoft.Euonia.Linq;

namespace Nerosoft.Euonia.Repository;

/// <summary>
/// Extensions methods for <see cref="IQueryable{TEntity}"/>.
/// </summary>
public static class RepositoryExtensions
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="source"></param>
    /// <param name="action"></param>
    /// <param name="page"></param>
    /// <param name="size"></param>
    /// <param name="cancellationToken"></param>
    /// <typeparam name="TEntity"></typeparam>
    /// <returns></returns>
    public static async Task<PageableCollection<TEntity>> GetPagedCollectionAsync<TEntity>(this IQueryable<TEntity> source, Func<IQueryable<TEntity>, CancellationToken, Task<IList<TEntity>>> action, int? page, int? size, CancellationToken cancellationToken = default)
    {
        var handler = new QueryHandler<TEntity>(source);

        handler.AddCriteria(t => true);
        var count = handler.GetCount();

        handler.SetPage(page ?? 1).SetSize(size ?? int.MaxValue);

        var list = await handler.QueryAsync(async query => await action(query, cancellationToken));

        return new PageableCollection<TEntity>(list) { TotalCount = count, PageNumber = page ?? 1, PageSize = size ?? int.MaxValue };
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="source"></param>
    /// <param name="action"></param>
    /// <param name="order"></param>
    /// <param name="page"></param>
    /// <param name="size"></param>
    /// <param name="cancellationToken"></param>
    /// <typeparam name="TEntity"></typeparam>
    /// <returns></returns>
    public static async Task<PageableCollection<TEntity>> GetPagedCollectionAsync<TEntity>(this IQueryable<TEntity> source, Func<IQueryable<TEntity>, CancellationToken, Task<IList<TEntity>>> action, Action<Orderable<TEntity>> order, int? page, int? size, CancellationToken cancellationToken = default)
    {
        var handler = new QueryHandler<TEntity>(source);

        handler.AddCriteria(t => true);
        var count = handler.GetCount();

        handler.SetPage(page ?? 1).SetSize(size ?? int.MaxValue);

        handler.SetCollator(order);

        var list = await handler.QueryAsync(async query => await action(query, cancellationToken));

        return new PageableCollection<TEntity>(list) { TotalCount = count, PageNumber = page ?? 1, PageSize = size ?? int.MaxValue };
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="source"></param>
    /// <param name="page"></param>
    /// <param name="size"></param>
    /// <param name="sort"></param>
    /// <typeparam name="TEntity"></typeparam>
    /// <returns></returns>
    public static IQueryable<TEntity> Paginate<TEntity>(this IQueryable<TEntity> source, int page, int size, Func<IQueryable<TEntity>, IQueryable<TEntity>> sort)
    {
        source = Sort(source, sort);
        return Paginate(source, page, size);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="source"></param>
    /// <param name="page"></param>
    /// <param name="size"></param>
    /// <param name="sorts"></param>
    /// <typeparam name="TEntity"></typeparam>
    /// <returns></returns>
    public static IQueryable<TEntity> Paginate<TEntity>(this IQueryable<TEntity> source, int page, int size, params string[] sorts)
    {
        source = Sort(source, sorts);
        return Paginate(source, page, size);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="source"></param>
    /// <param name="page"></param>
    /// <param name="size"></param>
    /// <param name="sorts"></param>
    /// <typeparam name="TEntity"></typeparam>
    /// <returns></returns>
    public static IQueryable<TEntity> Paginate<TEntity>(this IQueryable<TEntity> source, int page, int size, IDictionary<string, SortType> sorts)
    {
        source = Sort(source, sorts);

        return Paginate(source, page, size);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="source"></param>
    /// <param name="page"></param>
    /// <param name="size"></param>
    /// <typeparam name="TEntity"></typeparam>
    /// <returns></returns>
    public static IQueryable<TEntity> Paginate<TEntity>(this IQueryable<TEntity> source, int page, int size)
    {
        var pageIndex = Math.Max(1, page) - 1;
        var pageSize = Math.Min(int.MaxValue, size);

        var offset = pageIndex * pageSize;

        return source.Skip(offset).Take(pageSize);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="source"></param>
    /// <param name="sort"></param>
    /// <typeparam name="TEntity"></typeparam>
    /// <returns></returns>
    public static IQueryable<TEntity> Sort<TEntity>(this IQueryable<TEntity> source, Func<IQueryable<TEntity>, IQueryable<TEntity>> sort)
    {
        source = sort == null ? source : sort(source);
        return source;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="source"></param>
    /// <param name="sorts"></param>
    /// <typeparam name="TEntity"></typeparam>
    /// <returns></returns>
    public static IQueryable<TEntity> Sort<TEntity>(this IQueryable<TEntity> source, params string[] sorts)
    {
        if (sorts == null || sorts.Length == 0)
        {
            return source;
        }

        var sortDictionary = new Dictionary<string, SortType>();

        const string pattern = @"^([+-])?([A-z0-9_]+)$";

        foreach (var sort in sorts)
        {
            if (!Regex.IsMatch(sort, pattern))
            {
                continue;
            }

            var match = Regex.Match(sort, pattern);
            var propertyName = match.Groups[2].Value;

            if (sortDictionary.Any(t => t.Key.Equals(propertyName, StringComparison.OrdinalIgnoreCase)))
            {
                continue;
            }

            var sortType = match.Groups[1].Value switch
            {
                "+" => SortType.Ascending,
                "-" => SortType.Descending,
                _ => SortType.Unspecified
            };

            sortDictionary.Add(propertyName, sortType);
        }

        return Sort(source, sortDictionary);
    }

    /// <summary>
    /// Sots query.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="sorts"></param>
    /// <typeparam name="TEntity"></typeparam>
    /// <returns></returns>
    public static IQueryable<TEntity> Sort<TEntity>(this IQueryable<TEntity> source, IDictionary<string, SortType> sorts)
    {
        var hasOrder = false;

        foreach (var (key, value) in sorts)
        {
            var property = typeof(TEntity).GetProperty(key, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            if (property == null)
            {
                continue;
            }

            var parameterExpression = Expression.Parameter(typeof(TEntity), "sort");
            var memberExpression = Expression.MakeMemberAccess(parameterExpression, property);
            var lambdaExpression = Expression.Lambda(memberExpression, parameterExpression);

            var methodName = value switch
            {
                SortType.Unspecified => hasOrder ? nameof(Queryable.ThenBy) : nameof(Queryable.OrderBy),
                SortType.Ascending => hasOrder ? nameof(Queryable.ThenBy) : nameof(Queryable.OrderBy),
                SortType.Descending => hasOrder ? nameof(Queryable.ThenByDescending) : nameof(Queryable.OrderByDescending),
                _ => string.Empty
            };

            var expression = Expression.Call(typeof(Queryable), methodName, new[] { typeof(TEntity), property.PropertyType }, source.Expression, lambdaExpression);

            source = source.Provider.CreateQuery<TEntity>(expression);
            hasOrder = true;
        }

        return source;
    }
}