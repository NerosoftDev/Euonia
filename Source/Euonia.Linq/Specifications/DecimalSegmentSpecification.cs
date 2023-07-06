using System.Linq.Expressions;

namespace Nerosoft.Euonia.Linq;

/// <summary>
/// Represents a specification that can be used to filter a collection of objects that the <see cref="decimal"/> property value is in speficied range.
/// </summary>
/// <typeparam name="TTarget"></typeparam>
/// <typeparam name="TProperty"></typeparam>
public sealed class DecimalSegmentSpecification<TTarget, TProperty> : SegmentSpecification<TTarget, TProperty, decimal>
    where TTarget : class
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DecimalSegmentSpecification{TTarget, TProperty}"/> class.
    /// </summary>
    /// <param name="property"></param>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <param name="boundary"></param>
    public DecimalSegmentSpecification(Expression<Func<TTarget, TProperty>> property, decimal? min, decimal? max, RangeBoundary boundary) 
        : base(property, min, max, boundary)
    {
    }
}
