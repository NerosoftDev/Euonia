using System.Linq.Expressions;

namespace Nerosoft.Euonia.Linq;

/// <summary>
/// A logic AND Specification
/// </summary>
/// <typeparam name="T">Type of entity that check this specification</typeparam>
public sealed class AndSpecification<T> : Specification<T>
   where T : class
{
    #region Members

    private readonly ISpecification<T> _rightSideSpecification;
    private readonly ISpecification<T> _leftSideSpecification;

    #endregion

    #region Public Constructor

    /// <summary>
    /// Default constructor for AndSpecification
    /// </summary>
    /// <param name="leftSide">Left side specification</param>
    /// <param name="rightSide">Right side specification</param>
    public AndSpecification(ISpecification<T> leftSide, ISpecification<T> rightSide)
    {
        _leftSideSpecification = leftSide ?? throw new ArgumentNullException(nameof(leftSide));
        _rightSideSpecification = rightSide ?? throw new ArgumentNullException(nameof(rightSide));
    }

    #endregion

    #region Composite Specification overrides

    /// <summary>
    /// Left side specification for this composite element
    /// </summary>
    public ISpecification<T> Left => _leftSideSpecification;

    /// <summary>
    /// Right side specification for this composite element
    /// </summary>
    public ISpecification<T> Right => _rightSideSpecification;

    /// <summary>
    /// <see cref="ISpecification{T}"/>
    /// </summary>
    /// <returns><see cref="ISpecification{T}"/></returns>
    public override Expression<Func<T, bool>> Satisfy()
    {
        var left = _leftSideSpecification.Satisfy();
        var right = _rightSideSpecification.Satisfy();

        return (left.And(right));

    }

    #endregion
}
