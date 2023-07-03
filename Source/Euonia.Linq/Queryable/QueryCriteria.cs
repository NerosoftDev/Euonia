namespace Nerosoft.Euonia.Linq;

public sealed class QueryCriteria<TEntity>
    where TEntity : class
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="specification"></param>
    /// <param name="offset"></param>
    /// <param name="size"></param>
    public QueryCriteria(ISpecification<TEntity> specification, int offset, int size)
    {
        Specification = specification;
        this.Offset = offset;
        Size = size;
    }

    /// <summary>
    /// 
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
    /// 
    /// </summary>
    public ISpecification<TEntity> Specification { get; }

    /// <summary>
    /// 
    /// </summary>
    public Action<Orderable<TEntity>> Collation { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public int Offset { get; }

    /// <summary>
    /// 
    /// </summary>
    public int Size { get; }
}