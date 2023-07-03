using System.Linq.Expressions;

namespace Nerosoft.Euonia.Linq;

/// <summary>
/// NotEspecification convert a original
/// specification with NOT logic operator
/// </summary>
/// <typeparam name="TEntity">Type of element for this specificaiton</typeparam>
public sealed class NotSpecification<TEntity> : Specification<TEntity>
    where TEntity : class
{
    #region Members

    private readonly Expression<Func<TEntity, bool>> _predicate;

    #endregion

    #region Constructor

    /// <summary>
    /// Constructor for NotSpecificaiton
    /// </summary>
    /// <param name="originalSpecification">Original specification</param>
    public NotSpecification(ISpecification<TEntity> originalSpecification)
    {

        if (originalSpecification == null)
        {
            throw new ArgumentNullException(nameof(originalSpecification));
        }

        _predicate = originalSpecification.Satisfy();
    }

    /// <summary>
    /// Constructor for NotSpecification
    /// </summary>
    /// <param name="originalSpecification">Original specificaiton</param>
    public NotSpecification(Expression<Func<TEntity, bool>> originalSpecification)
    {
        _predicate = originalSpecification ?? throw new ArgumentNullException(nameof(originalSpecification));
    }

    #endregion

    #region Override Specification methods

    /// <summary>
    /// <see cref="ISpecification{TEntity}"/>
    /// </summary>
    /// <returns><see cref="ISpecification{TEntity}"/></returns>
    public override Expression<Func<TEntity, bool>> Satisfy()
    {
        return Expression.Lambda<Func<TEntity, bool>>(Expression.Not(_predicate.Body),
                                                     _predicate.Parameters.Single());
    }

    #endregion
}
