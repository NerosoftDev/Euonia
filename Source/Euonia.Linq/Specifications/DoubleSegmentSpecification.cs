using System.Linq.Expressions;

namespace Nerosoft.Euonia.Linq;

public sealed class DoubleSegmentSpecification<TTarget, TProperty> : SegmentSpecification<TTarget, TProperty, double>
    where TTarget : class
{
    public DoubleSegmentSpecification(Expression<Func<TTarget, TProperty>> property, double? min, double? max, RangeBoundary boundary)
        : base(property, min, max, boundary)
    {
    }
}
