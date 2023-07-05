namespace Nerosoft.Euonia.Threading;

public readonly struct TimeoutValue : IEquatable<TimeoutValue>, IComparable<TimeoutValue>
{
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

    public int InMilliseconds { get; }

    public int InSeconds => IsInfinite ? throw new InvalidOperationException("infinite timeout cannot be converted to seconds") : InMilliseconds / 1000;
    public bool IsInfinite => InMilliseconds == Timeout.Infinite;
    public bool IsZero => InMilliseconds == 0;
    public TimeSpan TimeSpan => TimeSpan.FromMilliseconds(InMilliseconds);

    public bool Equals(TimeoutValue that) => InMilliseconds == that.InMilliseconds;
    public override bool Equals(object obj) => obj is TimeoutValue that && Equals(that);
    public override int GetHashCode() => InMilliseconds;

    public int CompareTo(TimeoutValue that) => IsInfinite ? (that.IsInfinite ? 0 : 1) : that.IsInfinite ? -1 : InMilliseconds.CompareTo(that.InMilliseconds);

    public static bool operator ==(TimeoutValue a, TimeoutValue b) => a.Equals(b);
    public static bool operator !=(TimeoutValue a, TimeoutValue b) => !(a == b);

    public static implicit operator TimeoutValue(TimeSpan? timeout) => new TimeoutValue(timeout);

    public override string ToString() => IsInfinite ? "∞" : IsZero ? "0" : TimeSpan.ToString();
}