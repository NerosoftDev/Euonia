using System.Linq.Expressions;

namespace Nerosoft.Euonia.Linq;

public sealed class DecimalSegmentSpecification<TTarget, TProperty> : SegmentSpecification<TTarget, TProperty, decimal>
    where TTarget : class
{
    public DecimalSegmentSpecification(Expression<Func<TTarget, TProperty>> property, decimal? min, decimal? max, RangeBoundary boundary) 
        : base(property, min, max, boundary)
    {
    }
}
