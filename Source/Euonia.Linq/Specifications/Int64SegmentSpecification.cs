using System.Linq.Expressions;

namespace Nerosoft.Euonia.Linq;

/// <summary>
/// Represents a specification that can be used to filter a collection of objects that the <see cref="long"/> property value is in speficied range.
/// </summary>
/// <typeparam name="TTarget"></typeparam>
/// <typeparam name="TProperty"></typeparam>
public sealed class Int64SegmentSpecification<TTarget, TProperty> : SegmentSpecification<TTarget, TProperty, long>
    where TTarget : class
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Int64SegmentSpecification{TTarget, TProperty}"/> class.
    /// </summary>
    /// <param name="property"></param>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <param name="boundary"></param>
    public Int64SegmentSpecification(Expression<Func<TTarget, TProperty>> property, long? min, long? max, RangeBoundary boundary) 
        : base(property, min, max, boundary)
    {
    }
}
