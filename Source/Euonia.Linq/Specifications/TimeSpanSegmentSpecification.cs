using System.Linq.Expressions;

namespace Nerosoft.Euonia.Linq;

/// <summary>
/// Represents a specification that can be used to filter a collection of objects that the <see cref="TimeSpan"/> property value is in speficied range.
/// </summary>
/// <typeparam name="TTarget"></typeparam>
/// <typeparam name="TProperty"></typeparam>
public sealed class TimeSpanSegmentSpecification<TTarget, TProperty> : SegmentSpecification<TTarget, TProperty, TimeSpan>
    where TTarget : class
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TimeSpanSegmentSpecification{TTarget, TProperty}"/> class.
    /// </summary>
    /// <param name="property"></param>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <param name="boundary"></param>
    public TimeSpanSegmentSpecification(Expression<Func<TTarget, TProperty>> property, TimeSpan? min, TimeSpan? max, RangeBoundary boundary)
        : base(property, min, max, boundary)
    {
    }
}
