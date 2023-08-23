namespace Nerosoft.Euonia.Threading;

/// <summary>
/// Represents a timeout value for a task
/// </summary>
public readonly struct TimeoutValue : IEquatable<TimeoutValue>, IComparable<TimeoutValue>
{
    /// <summary>
    /// Initialize a new instance of <see cref="TimeoutValue"/> with the specified timeout value.
    /// </summary>
    /// <param name="timeout"></param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public TimeoutValue(TimeSpan? timeout)
    {
        if (timeout is { } timeoutValue)
        {
            // based on Task.Wait(TimeSpan) 
            // https://referencesource.microsoft.com/#mscorlib/system/threading/Tasks/Task.cs,855657030ba22f78

            var totalMilliseconds = (long)timeoutValue.TotalMilliseconds;
            if (totalMilliseconds is < -1 or > int.MaxValue)
            {
                var message = $"Must be {nameof(Timeout)}.{nameof(Timeout.InfiniteTimeSpan)} ({Timeout.InfiniteTimeSpan}) or a non-negative value <= {TimeSpan.FromMilliseconds(int.MaxValue)})";
                throw new ArgumentOutOfRangeException(nameof(timeout), timeoutValue, message);
            }

            InMilliseconds = (int)totalMilliseconds;
        }
        else
        {
            InMilliseconds = Timeout.Infinite;
        }
    }

    /// <summary>
    /// Gets the timeout value in milliseconds
    /// </summary>
    public int InMilliseconds { get; }

    /// <summary>
    /// Gets the timeout value in seconds
    /// </summary>
    public int InSeconds => IsInfinite ? throw new InvalidOperationException("infinite timeout cannot be converted to seconds") : InMilliseconds / 1000;

    /// <summary>
    /// Gets a value indicating whether the timeout is infinite
    /// </summary>
    public bool IsInfinite => InMilliseconds == Timeout.Infinite;
    
    /// <summary>
    /// Gets a value indicating whether the timeout is zero
    /// </summary>
    public bool IsZero => InMilliseconds == 0;
    
    /// <summary>
    /// Gets the timeout value as a <see cref="TimeSpan"/>
    /// </summary>
    public TimeSpan TimeSpan => TimeSpan.FromMilliseconds(InMilliseconds);

    /// <summary>
    /// Determines whether the specified <see cref="TimeoutValue"/> is equal to the current <see cref="TimeoutValue"/>.
    /// </summary>
    /// <param name="that"></param>
    /// <returns></returns>
    public bool Equals(TimeoutValue that) => InMilliseconds == that.InMilliseconds;

    /// <summary>
    /// Determines whether the specified <see cref="object"/> is equal to the current <see cref="TimeoutValue"/>.
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public override bool Equals(object obj) => obj is TimeoutValue that && Equals(that);

    /// <inheritdoc />
    public override int GetHashCode() => InMilliseconds;

    /// <summary>
    /// Compares the current <see cref="TimeoutValue"/> with another <see cref="TimeoutValue"/> 
    /// and returns an value of milliseconds that indicates whether the current instance precedes, follows, 
    /// or occurs in the same position in the sort order as the other <see cref="TimeoutValue"/>.
    /// </summary>
    /// <param name="that"></param>
    /// <returns></returns>
    public int CompareTo(TimeoutValue that) => IsInfinite ? (that.IsInfinite ? 0 : 1) : that.IsInfinite ? -1 : InMilliseconds.CompareTo(that.InMilliseconds);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public static bool operator ==(TimeoutValue a, TimeoutValue b) => a.Equals(b);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public static bool operator !=(TimeoutValue a, TimeoutValue b) => !(a == b);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="timeout"></param>
    /// <returns></returns>
    public static implicit operator TimeoutValue(TimeSpan? timeout) => new(timeout);

    /// <inheritdoc />
    public override string ToString() => IsInfinite ? "∞" : IsZero ? "0" : TimeSpan.ToString();
}