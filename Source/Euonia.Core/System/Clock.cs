using System.Runtime.CompilerServices;

namespace System;

/// <summary>
/// Time related helper.
/// </summary>
public static class Clock
{
    /// <summary>
    /// Number of ticks per millisecond.
    /// </summary>
    public const long TicksPerMillisecond = 10000;

    /// <summary>
    /// Ticks since 1970.
    /// </summary>
    public const long UnixEpochTicks = TimeSpan.TicksPerDay * DAYS_TO1970;

    /// <summary>
    /// Seconds since 1970.
    /// </summary>
    public const long UnixEpochSeconds = UnixEpochTicks / TimeSpan.TicksPerSecond;

    // Number of days in a non-leap year
    private const int DAYS_PER_YEAR = 365;

    // Number of days in 4 years
    private const int DAYS_PER4_YEARS = DAYS_PER_YEAR * 4 + 1;       // 1461

    // Number of days in 100 years
    private const int DAYS_PER100_YEARS = DAYS_PER4_YEARS * 25 - 1;  // 36524

    // Number of days in 400 years
    private const int DAYS_PER400_YEARS = DAYS_PER100_YEARS * 4 + 1; // 146097

    // Number of days from 1/1/0001 to 12/31/1969
    private const int DAYS_TO1970 = DAYS_PER400_YEARS * 4 + DAYS_PER100_YEARS * 3 + DAYS_PER4_YEARS * 17 + DAYS_PER_YEAR; // 719,162

    /// <summary>
    /// Computes a timestamp representing milliseconds since 1970.
    /// </summary>
    /// <returns>The milliseconds.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long GetUnixTimestampMillis()
    {
        return (DateTime.UtcNow.Ticks - UnixEpochTicks) / TicksPerMillisecond;
    }

    /// <summary>
    /// Computes a timestamp representing ticks since 1970.
    /// </summary>
    /// <returns>The ticks.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long GetUnixTimestampTicks()
    {
        return DateTime.UtcNow.Ticks - UnixEpochTicks;
    }

    /// <summary>
    /// Computes the milliseconds since 1970 up to the given <paramref name="date"/>.
    /// </summary>
    /// <param name="date">The <see cref="DateTime"/> base.</param>
    /// <returns>The milliseconds since 1970.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long ToUnixTimestampMillis(DateTime date)
    {
        return (date.Ticks - UnixEpochTicks) / TicksPerMillisecond;
    }
}
