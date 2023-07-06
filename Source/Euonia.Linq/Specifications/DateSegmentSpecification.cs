using System.Linq.Expressions;

namespace Nerosoft.Euonia.Linq;

/// <summary>
/// Represents a specification that can be used to filter a collection of objects that the <see cref="DateTime"/> property value is in speficied range.
/// </summary>
/// <typeparam name="TTarget"></typeparam>
/// <typeparam name="TProperty"></typeparam>
public sealed class DateSegmentSpecification<TTarget, TProperty> : SegmentSpecification<TTarget, TProperty, DateTime>
    where TTarget : class
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DateSegmentSpecification{TTarget, TProperty}"/> class.
    /// </summary>
    /// <param name="property"></param>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <param name="boundary"></param>
    public DateSegmentSpecification(Expression<Func<TTarget, TProperty>> property, DateTime? min, DateTime? max, RangeBoundary boundary)
        : base(property, min, max?.AddDays(1).Date, boundary)
    {
    }
}