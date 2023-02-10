namespace Nerosoft.Euonia.Collections;

/// <summary>
/// Class PageableCollection.
/// Implements the <see cref="List{T}" />
/// </summary>
/// <typeparam name="T"></typeparam>
/// <seealso cref="List{T}" />
public class PageableCollection<T> : List<T>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PageableCollection{T}"/> class.
    /// </summary>
    /// <param name="items">The items.</param>
    public PageableCollection(IEnumerable<T> items)
    {
        AddRange(items);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PageableCollection{T}"/> class.
    /// </summary>
    /// <param name="items">The items.</param>
    public PageableCollection(params T[] items)
    {
        AddRange(items);
    }

    #region IPageableCollection<T> Members

    /// <summary>
    /// Gets or sets the page number.
    /// </summary>
    /// <value>The page number.</value>
    public long PageNumber { get; set; }

    /// <summary>
    /// Gets or sets the size of the page.
    /// </summary>
    /// <value>The size of the page.</value>
    public long PageSize { get; set; }

    /// <summary>
    /// Gets or sets the total count.
    /// </summary>
    /// <value>The total count.</value>
    public long TotalCount { get; set; }

    /// <summary>
    /// Gets the page count.
    /// </summary>
    /// <value>The page count.</value>
    /// <exception cref="InvalidOperationException"></exception>
    public virtual long PageCount
    {
        get
        {
            if (PageSize <= 0)
            {
                throw new InvalidOperationException();
            }

            return (int)Math.Ceiling((double)TotalCount / PageSize);
        }
    }

    /// <summary>
    /// Gets the start position.
    /// </summary>
    /// <value>The start position.</value>
    public virtual long StartPosition => (PageNumber - 1) * PageSize + 1;

    /// <summary>
    /// Gets the end position.
    /// </summary>
    /// <value>The end position.</value>
    public virtual long EndPosition => PageNumber * PageSize > TotalCount ? TotalCount : PageNumber * PageSize;

    #endregion
}
