using System.Linq.Expressions;

namespace Nerosoft.Euonia.Linq;

public class SegmentSpecification<TTarget, TProperty, TValue> : ISpecification<TTarget>
    where TTarget : class
    where TValue : struct, IComparable<TValue>
{
    private readonly Expression<Func<TTarget, TProperty>> _property;

    private readonly PredicateExpressionBuilder<TTarget> _builder;

    private readonly RangeBoundary _boundary;

    /// <summary>
    /// Initialize a new instance which inherited <see cref="SegmentSpecification{TTarget, TProperty, TValue}"/>
    /// </summary>
    /// <param name="property"></param>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <param name="boundary"></param>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="ArgumentException"></exception>
    protected SegmentSpecification(Expression<Func<TTarget, TProperty>> property, TValue? min, TValue? max, RangeBoundary boundary)
    {
        _builder = new PredicateExpressionBuilder<TTarget>();
        _property = property ?? throw new ArgumentNullException(nameof(property), "Property expression can not null.");
        if (min == null && max == null)
        {
            throw new ArgumentNullException("min/max", $"At least one of {nameof(min)}/{nameof(max)} parameter is not null.");
        }
        else if (IsMinGreaterThanMax(min, max))
        {
            throw new ArgumentException($"Value of {min} can not greater than {max}.");
        }

        MinimumValue = GetValue(min);
        MaximumValue = GetValue(max);
        _boundary = boundary;
    }

    /// <summary>
    /// Gets the maximum value.
    /// </summary>
    protected TValue? MaximumValue { get; }

    /// <summary>
    /// Gets the minium value.
    /// </summary>
    protected TValue? MinimumValue { get; }

    /// <summary>
    /// Check whether the Min value is greater than the Max value or not.
    /// </summary>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <returns></returns>
    protected virtual bool IsMinGreaterThanMax(TValue? min, TValue? max)
    {
        if (min == null || max == null)
        {
            return false;
        }
        return min.Value.CompareTo(max.Value) > 0;
    }

    protected virtual TValue? GetValue(TValue? value)
    {
        return value;
    }

    /// <summary>
    /// Gets the query operator for min value.
    /// </summary>
    /// <param name="boundary"></param>
    /// <returns></returns>
    protected virtual QueryOperator GetMinValueOperator(RangeBoundary boundary)
    {
#pragma warning disable IDE0066 // 将 switch 语句转换为表达式
        switch (boundary)
        {
            case RangeBoundary.Left:
            case RangeBoundary.Both:
                return QueryOperator.GreaterThanOrEqual;
            default:
                return QueryOperator.GreaterThan;
        }
#pragma warning restore IDE0066 // 将 switch 语句转换为表达式
    }

    /// <summary>
    /// Gets the query operator for max value.
    /// </summary>
    /// <param name="boundary"></param>
    /// <returns></returns>
    protected virtual QueryOperator GetMaxValueOperator(RangeBoundary boundary)
    {
#pragma warning disable IDE0066
        switch (boundary)
        {
            case RangeBoundary.Right:
            case RangeBoundary.Both:
                return QueryOperator.LessThanOrEqual;
            default:
                return QueryOperator.LessThan;
        }
#pragma warning restore IDE0066
    }

    /// <inheritdoc />
    public virtual Expression<Func<TTarget, bool>> Satisfy()
    {
        if (MinimumValue != null)
        {
            _builder.Append(_property, GetMinValueOperator(_boundary), MinimumValue);
        }

        if (MaximumValue != null)
        {
            _builder.Append(_property, GetMaxValueOperator(_boundary), MaximumValue);
        }

        return _builder.ToLambda();
    }
}
