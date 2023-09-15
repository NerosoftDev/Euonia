using System.Linq.Expressions;

namespace Nerosoft.Euonia.Linq;

/// <summary>
/// Represents a specification which can be used to filter a collection of objects.
/// </summary>
/// <typeparam name="TTarget"></typeparam>
/// <typeparam name="TProperty"></typeparam>
/// <typeparam name="TValue"></typeparam>
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
		_property = property ?? throw new ArgumentNullException(nameof(property), Resources.IDS_PROPERTY_EXPRESSION_CAN_NOT_NULL);
		if (min == null && max == null)
		{
			// ReSharper disable once NotResolvedInText
			throw new ArgumentNullException("min/max", string.Format(Resources.IDS_AT_LEAST_ONE_PARAMETER_IS_REQUIRED, $"{nameof(min)}/{nameof(max)}"));
		}

		if (IsMinGreaterThanMax(min, max))
		{
			throw new ArgumentException(string.Format(Resources.IDS_VALUE_OF_MIN_CAN_NOT_GREATER_THAN_MAX, min, max));
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

	/// <summary>
	/// Gets the value.
	/// </summary>
	/// <param name="value"></param>
	/// <returns></returns>
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