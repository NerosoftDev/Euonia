namespace Nerosoft.Euonia.Linq;

/// <summary>
/// Represents the sort order in a sorted query.
/// </summary>
public enum SortType
{
    /// <summary>
    /// Indicates that the sort is unspecified.
    /// </summary>
    Unspecified = 0,

    /// <summary>
    /// Indicates an ascending sort.
    /// </summary>
    Ascending = -1,

    /// <summary>
    /// Indicates a descending sort.
    /// </summary>
    Descending = 1
}
