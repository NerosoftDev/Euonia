using System.Linq.Expressions;

namespace Nerosoft.Euonia.Linq;

public sealed class Int32SegmentSpecification<TTarget, TProperty> : SegmentSpecification<TTarget, TProperty, int>
    where TTarget : class
{
    public Int32SegmentSpecification(Expression<Func<TTarget, TProperty>> property, int? min, int? max, RangeBoundary boundary) 
        : base(property, min, max, boundary)
    {
    }
}
