namespace Nerosoft.Euonia.Linq;

/// <summary>
/// Qquery criteria for a given generic entity.
/// </summary>
/// <typeparam name="TEntity">The entity type.</typeparam>
public sealed class QueryCriteria<TEntity>
    where TEntity : class
{
    /// <summary>
    /// Initializes a new instance of the <see cref="QueryCriteria{TEntity}"/> class for a specified specification, offset, and size.
    /// </summary>
    /// <param name="specification"></param>
    /// <param name="offset"></param>
    /// <param name="size"></param>
    public QueryCriteria(ISpecification<TEntity> specification, int offset, int size)
    {
        Specification = specification;
        Offset = offset;
        Size = size;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="QueryCriteria{TEntity}"/> class for a specified specification, collation, offset, and size.
    /// </summary>
    /// <param name="specification"></param>
    /// <param name="collation"></param>
    /// <param name="offset"></param>
    /// <param name="size"></param>
    public QueryCriteria(ISpecification<TEntity> specification, Action<Orderable<TEntity>> collation, int offset, int size)
        : this(specification, offset, size)
    {
        Collation = collation;
    }

    /// <summary>
    /// Gets the Specification property that represents the query specification.
    /// </summary>
    public ISpecification<TEntity> Specification { get; }

    /// <summary>
    /// Gets or sets an action to collate elements before they're returned.
    /// </summary>
    public Action<Orderable<TEntity>> Collation { get; set; }

    /// <summary>
    /// Gets the Offset property that represents the query offset.
    /// </summary>
    public int Offset { get; }

    /// <summary>
    /// Gets the Size property that represents the query size.
    /// </summary>
    public int Size { get; }
}