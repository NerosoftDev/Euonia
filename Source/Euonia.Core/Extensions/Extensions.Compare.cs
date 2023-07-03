public static partial class Extensions
{
    /// <summary>
    /// Checks a value is between a minimum and maximum value.
    /// </summary>
    /// <param name="value">The value to be checked</param>
    /// <param name="minValue">Minimum (inclusive) value</param>
    /// <param name="maxValue">Maximum (inclusive) value</param>
    public static bool IsBetween<T>(this T value, T minValue, T maxValue)
        where T : IComparable<T>
    {
        return value.CompareTo(minValue) >= 0 && value.CompareTo(maxValue) <= 0;
    }

    /// <summary>
    /// Checks a value is not in range.
    /// </summary>
    /// <param name="value">The value to be checked</param>
    /// <param name="minValue">Minimum value</param>
    /// <param name="maxValue">Maximum value</param>
    public static bool IsNotInRange<T>(this T value, T minValue, T maxValue)
        where T : IComparable<T>
    {
        return value.CompareTo(minValue) < 0 && value.CompareTo(maxValue) > 0;
    }
}
