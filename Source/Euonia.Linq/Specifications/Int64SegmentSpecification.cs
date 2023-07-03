using System.Linq.Expressions;

namespace Nerosoft.Euonia.Linq;

public sealed class Int64SegmentSpecification<TTarget, TProperty> : SegmentSpecification<TTarget, TProperty, long>
    where TTarget : class
{
    public Int64SegmentSpecification(Expression<Func<TTarget, TProperty>> property, long? min, long? max, RangeBoundary boundary) 
        : base(property, min, max, boundary)
    {
    }
}
