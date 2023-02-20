using System.Linq.Expressions;

namespace Nerosoft.Euonia.Linq;

/// <summary>
/// A Logic OR Specification
/// </summary>
/// <typeparam name="T">Type of entity that check this specification</typeparam>
public sealed class OrSpecification<T> : Specification<T>
    where T : class
{
    #region Members

    private readonly ISpecification<T> _right;
    private readonly ISpecification<T> _left;

    #endregion

    #region Public Constructor

    /// <summary>
    /// Default constructor for OrSpecification
    /// </summary>
    /// <param name="leftSide">Left side specification</param>
    /// <param name="rightSide">Right side specification</param>
    public OrSpecification(ISpecification<T> leftSide, ISpecification<T> rightSide)
    {
        _left = leftSide ?? throw new ArgumentNullException(nameof(leftSide));
        _right = rightSide ?? throw new ArgumentNullException(nameof(rightSide));
    }

    #endregion

    #region Composite Specification overrides

    /// <summary>
    /// Left side specification for this composite element
    /// </summary>
    public ISpecification<T> Left => _left;

    /// <summary>
    /// Right side specification for this composite element
    /// </summary>
    public ISpecification<T> Right => _right;

    /// <summary>
    /// <see cref="ISpecification{T}"/>
    /// </summary>
    /// <returns><see cref="ISpecification{T}"/></returns>
    public override Expression<Func<T, bool>> Satisfy()
    {
        var left = _left.Satisfy();
        var right = _right.Satisfy();

        return (left.Or(right));

    }

    #endregion
}
