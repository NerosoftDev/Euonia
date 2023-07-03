using System.Linq.Expressions;

namespace Nerosoft.Euonia.Linq;

public sealed class DateSegmentSpecification<TTarget, TProperty> : SegmentSpecification<TTarget, TProperty, DateTime>
    where TTarget : class
{
    public DateSegmentSpecification(Expression<Func<TTarget, TProperty>> property, DateTime? min, DateTime? max, RangeBoundary boundary)
        : base(property, min, max?.AddDays(1).Date, boundary)
    {
    }
}