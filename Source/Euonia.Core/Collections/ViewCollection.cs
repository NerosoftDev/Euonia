namespace Nerosoft.Euonia.Collections;

/// <summary>
/// To be added.
/// </summary>
/// <typeparam name="T"></typeparam>
public class ViewCollection<T> 
    where T : class, new()
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ViewCollection{T}"/> class.
    /// </summary>
    public ViewCollection()
    {
        Items = new List<T>();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ViewCollection{T}"/> class.
    /// </summary>
    /// <param name="items">The items.</param>
    public ViewCollection(IList<T> items)
    {
        Items = new List<T>(items);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ViewCollection{T}"/> class.
    /// </summary>
    /// <param name="items">The items.</param>
    /// <param name="totalCount">The total count.</param>
    public ViewCollection(IList<T> items, long totalCount)
        : this(items)
    {
        TotalCount = totalCount;
    }

    /// <summary>
    /// Gets the items.
    /// </summary>
    /// <value>The items.</value>
    public ICollection<T> Items { get; }

    /// <summary>
    /// Gets or sets the total count.
    /// </summary>
    /// <value>The total count.</value>
    public long TotalCount { get; set; }
}