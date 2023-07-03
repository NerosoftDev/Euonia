/// <summary>
/// 
/// </summary>
/// <typeparam name="TValue"></typeparam>
public sealed class CheckResult<TValue>
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    internal CheckResult(TValue value)
    {
        Value = value;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    /// <param name="isValid"></param>
    internal CheckResult(TValue value, bool isValid)
        : this(value)
    {
        IsValid = isValid;
    }

    /// <summary>
    /// 
    /// </summary>
    public TValue Value { get; }

    /// <summary>
    /// 
    /// </summary>
    public bool IsValid { get; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    public static bool operator ==(CheckResult<TValue> left, CheckResult<TValue> right)
    {
        if (left is null || right is null)
        {
            return false;
        }

        return EqualityComparer<TValue>.Default.Equals(left.Value, right.Value);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    public static bool operator !=(CheckResult<TValue> left, CheckResult<TValue> right)
    {
        return !(left == right);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static implicit operator CheckResult<TValue>(TValue value)
    {
        return new CheckResult<TValue>(value);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="result"></param>
    /// <returns></returns>
    public static implicit operator TValue(CheckResult<TValue> result)
    {
        return result.Value;
    }

    /// <inheritdoc />
    public override bool Equals(object obj)
    {
        return obj switch
        {
            null => false,
            CheckResult<TValue> other => EqualityComparer<TValue>.Default.Equals(Value, other.Value),
            TValue other => EqualityComparer<TValue>.Default.Equals(Value, other),
            _ => false
        };
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return HashCode.Combine(Value);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="action"></param>
    /// <returns></returns>
    public CheckResult<TValue> Success(Action<TValue> action)
    {
        if (IsValid)
        {
            action?.Invoke(Value);
        }

        return this;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="action"></param>
    /// <returns></returns>
    public CheckResult<TValue> Failure(Action<TValue> action)
    {
        if (!IsValid)
        {
            action?.Invoke(Value);
        }

        return this;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="action"></param>
    /// <returns></returns>
    public void Then(Action<TValue, bool> action)
    {
        action?.Invoke(Value, IsValid);
    }
}
