using System.Linq.Expressions;

namespace Nerosoft.Euonia.Linq;

/// <summary>
/// A Direct Specification is a simple implementation
/// of specification that acquire this from a lambda expression
/// in  constructor
/// </summary>
/// <typeparam name="TEntity">Type of entity that check this specification</typeparam>
public sealed class DirectSpecification<TEntity> : Specification<TEntity>
    where TEntity : class
{
    #region Members

    private readonly Expression<Func<TEntity, bool>> _predicate;

    #endregion

    #region Constructor

    /// <summary>
    /// Default constructor for Direct Specification
    /// </summary>
    /// <param name="predicate">A Matching QueryCriteria</param>
    public DirectSpecification(Expression<Func<TEntity, bool>> predicate)
    {
        _predicate = predicate ?? throw new ArgumentNullException(nameof(predicate));
    }

    #endregion

    #region Override

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public override Expression<Func<TEntity, bool>> Satisfy()
    {
        return _predicate;
    }

    #endregion
}