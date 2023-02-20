using System.Linq.Expressions;

namespace Nerosoft.Euonia.Linq;

public sealed class DateTimeSegmentSpecification<TTarget, TProperty> : SegmentSpecification<TTarget, TProperty, DateTime>
    where TTarget : class
{
    public DateTimeSegmentSpecification(Expression<Func<TTarget, TProperty>> property, DateTime? min, DateTime? max, RangeBoundary boundary)
        : base(property, min, max, boundary)
    {
    }
}
