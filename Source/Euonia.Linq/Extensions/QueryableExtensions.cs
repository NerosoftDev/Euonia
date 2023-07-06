using System.Linq.Expressions;
using System.Reflection;
using Nerosoft.Euonia.Reflection;

namespace Nerosoft.Euonia.Linq;

/// <summary>
/// Extensions for <see cref="IQueryable{T}"/>.
/// </summary>
public static class QueryableExtensions
{
    /// <summary>
    /// Adds query criteria.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <param name="source">The query source,</param>
    /// <param name="criteria">The query criteria.</param>
    public static IQueryable<TEntity> Where<TEntity>(this IQueryable<TEntity> source, ISpecification<TEntity> criteria)
        where TEntity : class
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));
        if (criteria == null)
            throw new ArgumentNullException(nameof(criteria));
        var predicate = criteria.Satisfy();
        if (predicate == null)
            return source;
        return source.Where(predicate);
    }

    /// <summary>
    /// Adds query criteria if meet the condition.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <param name="source">The query source.</param>
    /// <param name="predicate">The query criteria.</param>
    /// <param name="condition">The criteria would be applied if <c><see langword="true"/></c>, or ignored if <c><see langword="false"/></c></param>
    public static IQueryable<TEntity> WhereIf<TEntity>(this IQueryable<TEntity> source, Expression<Func<TEntity, bool>> predicate, bool condition) where TEntity : class
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));
        if (condition == false)
            return source;
        return source.Where(predicate);
    }

    /// <summary>
    /// Adds query criteria if the predicate argument is not <see langword="null"/>.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <param name="source">The query source.</param>
    /// <param name="predicate">The query predicate.</param>
    /// <remarks>
    /// NOTE: Only one ctiteria property is allowd.
    /// e.g.
    /// <code>t => t.Name == "a"</code> -- YES
    /// <code>t => t.Name == "a" &amp;&amp; t.Mobile == "123"</code> --NO
    /// <code>t => t.Name == ""</code> -- IGNORED
    /// </remarks>
    public static IQueryable<TEntity> WhereIfNotEmpty<TEntity>(this IQueryable<TEntity> source, Expression<Func<TEntity, bool>> predicate) where TEntity : class
    {
        if (source == null)
        {
            throw new ArgumentNullException(nameof(source));
        }
        predicate = GetWhereIfNotEmptyExpression(predicate);
        if (predicate == null)
        {
            return source;
        }
        return source.Where(predicate);
    }

    /// <summary>
    /// Adds boundary query criteria.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <param name="source">The query source.</param>
    /// <typeparam name="TProperty">The property type.</typeparam>
    /// <param name="propertyExpression">The property expression. e.g. t => t.Age</param>
    /// <param name="min">The minimum boundary value.</param>
    /// <param name="max">The maximum boundary value.</param>
    /// <param name="boundary">The value to indicate whether the boundary value should be included or not.</param>
    public static IQueryable<TEntity> Between<TEntity, TProperty>(this IQueryable<TEntity> source, Expression<Func<TEntity, TProperty>> propertyExpression, int? min, int? max, RangeBoundary boundary = RangeBoundary.Both) where TEntity : class
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));
        return source.Where(new Int32SegmentSpecification<TEntity, TProperty>(propertyExpression, min, max, boundary));
    }

    /// <summary>
    /// Adds boundary query criteria.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <param name="source">The query source.</param>
    /// <typeparam name="TProperty">The property type.</typeparam>
    /// <param name="propertyExpression">The property expression. e.g. t => t.Age</param>
    /// <param name="min">The minimum boundary value.</param>
    /// <param name="max">The maximum boundary value.</param>
    /// <param name="boundary">The value to indicate whether the boundary value should be included or not.</param>
    public static IQueryable<TEntity> Between<TEntity, TProperty>(this IQueryable<TEntity> source, Expression<Func<TEntity, TProperty>> propertyExpression, double? min, double? max, RangeBoundary boundary = RangeBoundary.Both) where TEntity : class
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));
        return source.Where(new DoubleSegmentSpecification<TEntity, TProperty>(propertyExpression, min, max, boundary));
    }

    /// <summary>
    /// Adds boundary query criteria.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <param name="source">The query source.</param>
    /// <typeparam name="TProperty">The property type.</typeparam>
    /// <param name="propertyExpression">The property expression. e.g. t => t.Price</param>
    /// <param name="min">The minimum boundary value.</param>
    /// <param name="max">The maximum boundary value.</param>
    /// <param name="boundary">The value to indicate whether the boundary value should be included or not.</param>
    public static IQueryable<TEntity> Between<TEntity, TProperty>(this IQueryable<TEntity> source, Expression<Func<TEntity, TProperty>> propertyExpression, decimal? min, decimal? max, RangeBoundary boundary = RangeBoundary.Both) where TEntity : class
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));
        return source.Where(new DecimalSegmentSpecification<TEntity, TProperty>(propertyExpression, min, max, boundary));
    }

    /// <summary>
    /// Adds boundary query criteria.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <param name="source">The query source.</param>
    /// <typeparam name="TProperty">The property type.</typeparam>
    /// <param name="propertyExpression">The property expression. e.g. t => t.Time</param>
    /// <param name="min">The minimum boundary value.</param>
    /// <param name="max">The maximum boundary value.</param>
    /// <param name="includeTime">The value to indicate whether the time part should be included or not.</param>
    /// <param name="boundary">The value to indicate whether the boundary value should be included or not.</param>
    public static IQueryable<TEntity> Between<TEntity, TProperty>(this IQueryable<TEntity> source, Expression<Func<TEntity, TProperty>> propertyExpression, DateTime? min, DateTime? max, bool includeTime = true, RangeBoundary? boundary = null) where TEntity : class
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));
        if (includeTime)
            return source.Where(new DateTimeSegmentSpecification<TEntity, TProperty>(propertyExpression, min, max, boundary ?? RangeBoundary.Both));
        return source.Where(new DateSegmentSpecification<TEntity, TProperty>(propertyExpression, min, max, boundary ?? RangeBoundary.Left));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public static Expression<Func<TEntity, bool>> GetWhereIfNotEmptyExpression<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : class
    {
        if (predicate == null)
        {
            return null;
        }
        if (Lambda.GetConditionCount(predicate) > 1)
        {
            throw new InvalidOperationException(string.Format("Ony one predicate is allowed: {0}", predicate));
        }
        var value = predicate.Value();
        if (string.IsNullOrWhiteSpace(value?.ToString()))
        {
            return null;
        }
        return predicate;
    }

    /// <summary>
    /// Sorts the elements of a sequence in ascending order according to the specified property.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="propertyName"></param>
    /// <typeparam name="TEntity"></typeparam>
    /// <returns></returns>
    public static IQueryable<TEntity> OrderByAscending<TEntity>(this IQueryable<TEntity> source, string propertyName)
        where TEntity : class
    {
        return source.OrderBy(propertyName, SortType.Ascending);
    }

    /// <summary>
    /// Sorts the elements of a sequence in descending order according to the specified property.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="propertyName"></param>
    /// <typeparam name="TEntity"></typeparam>
    /// <returns></returns>
    public static IQueryable<TEntity> OrderByDescending<TEntity>(this IQueryable<TEntity> source, string propertyName)
        where TEntity : class
    {
        return source.OrderBy(propertyName, SortType.Descending);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="source"></param>
    /// <param name="propertyName"></param>
    /// <param name="sortType"></param>
    /// <typeparam name="TEntity"></typeparam>
    /// <returns></returns>
    public static IQueryable<TEntity> OrderBy<TEntity>(this IQueryable<TEntity> source, string propertyName, SortType sortType)
        where TEntity : class
    {
        var expression = PropertyAccessorCache<TEntity>.Get(propertyName);
        if (expression == null)
        {
            return source;
        }

        var methodName = sortType switch
        {
            SortType.Ascending => nameof(Queryable.OrderBy),
            SortType.Descending => nameof(Queryable.OrderByDescending),
            _ => string.Empty
        };

        if (string.IsNullOrEmpty(methodName))
        {
            return source;
        }

        var resultExpression = Expression.Call(typeof(Queryable), methodName, new[] { typeof(TEntity), expression.ReturnType },
            source.Expression,
            Expression.Quote(expression));
        return source.Provider.CreateQuery<TEntity>(resultExpression);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="source"></param>
    /// <param name="propertyName"></param>
    /// <param name="propertyValue"></param>
    /// <typeparam name="TEntity"></typeparam>
    /// <returns></returns>
    public static IQueryable<TEntity> Where<TEntity>(this IQueryable<TEntity> source, string propertyName, object propertyValue)
        where TEntity : class
    {
        return source.Where(propertyName, propertyValue, Expression.Equal);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="source"></param>
    /// <param name="propertyName"></param>
    /// <param name="propertyValue"></param>
    /// <param name="valueExpression"></param>
    /// <typeparam name="TEntity"></typeparam>
    /// <returns></returns>
    public static IQueryable<TEntity> Where<TEntity>(this IQueryable<TEntity> source, string propertyName, object propertyValue, Func<Expression, ConstantExpression, Expression> valueExpression)
        where TEntity : class
    {
        // 1. Retrieve member access expression
        var memberExpression = PropertyAccessorCache<TEntity>.Get(propertyName);
        if (memberExpression == null)
        {
            return source;
        }

        // 2. Try converting value to correct type
        object value;
        try
        {
            value = Convert.ChangeType(propertyValue, memberExpression.ReturnType);
        }
        catch (InvalidCastException)
        {
            return source;
        }
        catch (FormatException)
        {
            return source;
        }
        catch (OverflowException)
        {
            return source;
        }
        catch (ArgumentNullException)
        {
            return source;
        }
        catch (SystemException)
        {
            return source;
        }

        // 3. Construct expression tree
        var calculateExpression = valueExpression(memberExpression.Body, Expression.Constant(value, memberExpression.ReturnType));

        var expression = Expression.Lambda(calculateExpression, memberExpression.Parameters[0]);

        // 4. Construct new query
        var resultExpression = Expression.Call(
            null,
            GetMethodInfo(Queryable.Where, source, (Expression<Func<TEntity, bool>>)null),
            new[] { source.Expression, Expression.Quote(expression) });
        return source.Provider.CreateQuery<TEntity>(resultExpression);

        // ReSharper disable UnusedParameter.Local
        static MethodInfo GetMethodInfo<T1, T2, T3>(Func<T1, T2, T3> function, T1 t1, T2 t2)
        {
            return function.Method;
        }
        // ReSharper restore UnusedParameter.Local
    }
}