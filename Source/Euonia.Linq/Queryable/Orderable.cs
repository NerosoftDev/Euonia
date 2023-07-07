using System.Linq.Expressions;

namespace Nerosoft.Euonia.Linq;

/// <summary>
/// Represents a class that can be used to order a queryable sequence.
/// </summary>
/// <typeparam name="T"></typeparam>
public sealed class Orderable<T>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Orderable{T}"/> class.
    /// </summary>
    /// <param name="enumerable"></param>
    public Orderable(IQueryable<T> enumerable)
    {
        Queryable = enumerable;
    }

    /// <summary>
    /// Gets or sets the queryable sequence.
    /// </summary>
    public IQueryable<T> Queryable { get; private set; }

    /// <summary>
    /// Sorts the elements of a sequence in ascending order by using a specified comparer.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <param name="keySelector"></param>
    /// <returns></returns>
    public Orderable<T> Ascending<TKey>(Expression<Func<T, TKey>> keySelector)
    {
        Queryable = Queryable.OrderBy(keySelector);
        return this;
    }

    /// <summary>
    /// Sorts the elements of a sequence in ascending order by using a specified comparer.
    /// </summary>
    /// <typeparam name="TKey1"></typeparam>
    /// <typeparam name="TKey2"></typeparam>
    /// <param name="keySelector1"></param>
    /// <param name="keySelector2"></param>
    /// <returns></returns>
    public Orderable<T> Ascending<TKey1, TKey2>(Expression<Func<T, TKey1>> keySelector1, Expression<Func<T, TKey2>> keySelector2)
    {
        Queryable = Queryable.OrderBy(keySelector1)
                             .ThenBy(keySelector2);
        return this;
    }

    /// <summary>
    /// Sorts the elements of a sequence in ascending order by using a specified comparer.
    /// </summary>
    /// <typeparam name="TKey1"></typeparam>
    /// <typeparam name="TKey2"></typeparam>
    /// <typeparam name="TKey3"></typeparam>
    /// <param name="keySelector1"></param>
    /// <param name="keySelector2"></param>
    /// <param name="keySelector3"></param>
    /// <returns></returns>
    public Orderable<T> Ascending<TKey1, TKey2, TKey3>(Expression<Func<T, TKey1>> keySelector1, Expression<Func<T, TKey2>> keySelector2, Expression<Func<T, TKey3>> keySelector3)
    {
        Queryable = Queryable.OrderBy(keySelector1)
                             .ThenBy(keySelector2)
                             .ThenBy(keySelector3);
        return this;
    }

    /// <summary>
    /// Sorts the elements of a sequence in descending order by using a specified comparer.
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <param name="keySelector"></param>
    /// <returns></returns>
    public Orderable<T> Descending<TKey>(Expression<Func<T, TKey>> keySelector)
    {
        Queryable = Queryable.OrderByDescending(keySelector);
        return this;
    }

    /// <summary>
    /// Sorts the elements of a sequence in descending order by using a specified comparer.
    /// </summary>
    /// <typeparam name="TKey1"></typeparam>
    /// <typeparam name="TKey2"></typeparam>
    /// <param name="keySelector1"></param>
    /// <param name="keySelector2"></param>
    /// <returns></returns>
    public Orderable<T> Descending<TKey1, TKey2>(Expression<Func<T, TKey1>> keySelector1, Expression<Func<T, TKey2>> keySelector2)
    {
        Queryable = Queryable.OrderByDescending(keySelector1)
                             .ThenByDescending(keySelector2);
        return this;
    }

    /// <summary>
    /// Sorts the elements of a sequence in descending order by using a specified comparer.
    /// </summary>
    /// <typeparam name="TKey1"></typeparam>
    /// <typeparam name="TKey2"></typeparam>
    /// <typeparam name="TKey3"></typeparam>
    /// <param name="keySelector1"></param>
    /// <param name="keySelector2"></param>
    /// <param name="keySelector3"></param>
    /// <returns></returns>
    public Orderable<T> Descending<TKey1, TKey2, TKey3>(Expression<Func<T, TKey1>> keySelector1, Expression<Func<T, TKey2>> keySelector2, Expression<Func<T, TKey3>> keySelector3)
    {
        Queryable = Queryable.OrderByDescending(keySelector1)
                             .ThenByDescending(keySelector2)
                             .ThenByDescending(keySelector3);
        return this;
    }
}
