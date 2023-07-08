using Nerosoft.Euonia.Mapping;

/// <summary>
/// Extensions methods for <see cref="TypeAdapterFactory" />.
/// </summary>
public static partial class Extensions
{
    /// <summary>
    /// Project object to <typeparamref name="TDestination"/>
    /// </summary>
    /// <typeparam name="TDestination">The dto projection</typeparam>
    /// <param name="source">The source entity to project</param>
    /// <returns>The projected type</returns>
    public static TDestination ProjectedAs<TDestination>(this object source)
        where TDestination : class
    {
        var adapter = TypeAdapterFactory.CreateAdapter();
        return adapter.Adapt<TDestination>(source);
    }

    /// <summary>
    /// Project object to destination tyoe.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="destinationType"></param>
    /// <returns></returns>
    public static object ProjectedAs(this object source, Type destinationType)
    {
        var adapter = TypeAdapterFactory.CreateAdapter();
        return adapter.Adapt(source, destinationType);
    }

    /// <summary>
    /// Projected a enumerable collection of items
    /// </summary>
    /// <typeparam name="TDestination">The projection type</typeparam>
    /// <param name="items">the collection of entity items</param>
    /// <returns>Projected collection</returns>
    public static List<TDestination> ProjectedAsCollection<TDestination>(this IEnumerable<object> items)
        where TDestination : class
    {
        var adapter = TypeAdapterFactory.CreateAdapter();
        return adapter.Adapt<List<TDestination>>(items);
    }
}