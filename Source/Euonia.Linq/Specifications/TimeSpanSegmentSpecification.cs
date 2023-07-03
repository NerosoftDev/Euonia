using System.Linq.Expressions;

namespace Nerosoft.Euonia.Linq;

public sealed class TimeSpanSegmentSpecification<TTarget, TProperty> : SegmentSpecification<TTarget, TProperty, TimeSpan>
    where TTarget : class
{
    public TimeSpanSegmentSpecification(Expression<Func<TTarget, TProperty>> property, TimeSpan? min, TimeSpan? max, RangeBoundary boundary)
        : base(property, min, max, boundary)
    {
    }
}
