namespace Nerosoft.Euonia.Linq;

/// <summary>
/// Exrensions methods to combines specifications.
/// </summary>
public static class SpecificationExtensions
{
    /// <summary>
    /// Combines two specifications using the logical AND operator.
    /// </summary>
    /// <param name="first"></param>
    /// <param name="second"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static Specification<T> And<T>(this ISpecification<T> first, ISpecification<T> second)
        where T : class
    {
        var specification = (Specification<T>)first;
        specification &= (Specification<T>)second;
        return specification;
    }

    /// <summary>
    /// Combines two specifications using the logical AND operator if the condition is true.
    /// </summary>
    /// <param name="first"></param>
    /// <param name="condition"></param>
    /// <param name="second"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static Specification<T> AndIf<T>(this ISpecification<T> first, bool condition, Func<ISpecification<T>> second)
        where T : class
    {
        var specification = (Specification<T>)first;
        return !condition ? specification : specification.And(second());
    }

    /// <summary>
    /// Combines two specifications using the logical AND operator if the condition is true.
    /// </summary>
    /// <param name="first"></param>
    /// <param name="selector"></param>
    /// <param name="second"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static Specification<T> AndIf<T>(this ISpecification<T> first, Func<bool> selector, Func<ISpecification<T>> second)
        where T : class
    {
        if (selector == null)
        {
            throw new ArgumentNullException(nameof(selector));
        }

        var specification = (Specification<T>)first;
        var condition = selector();
        return !condition ? specification : specification.And(second());
    }
}
