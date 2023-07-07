using System.Linq.Expressions;

namespace Nerosoft.Euonia.Linq;

/// <summary>
/// Represents a specification that can be used to filter a collection of objects that the <see cref="int"/> property value is in speficied range.
/// </summary>
/// <typeparam name="TTarget"></typeparam>
/// <typeparam name="TProperty"></typeparam>
public sealed class Int32SegmentSpecification<TTarget, TProperty> : SegmentSpecification<TTarget, TProperty, int>
    where TTarget : class
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Int32SegmentSpecification{TTarget, TProperty}"/> class.
    /// </summary>
    /// <param name="property"></param>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <param name="boundary"></param>
    public Int32SegmentSpecification(Expression<Func<TTarget, TProperty>> property, int? min, int? max, RangeBoundary boundary) 
        : base(property, min, max, boundary)
    {
    }
}
