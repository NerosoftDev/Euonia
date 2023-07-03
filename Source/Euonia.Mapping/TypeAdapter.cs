namespace Nerosoft.Euonia.Mapping;

public class TypeAdapter
{
    /// <summary>
    /// Project <typeparamref name="TSource"/> to a new instance of type <typeparamref name="TDestination"/>
    /// </summary>
    /// <typeparam name="TSource"></typeparam>
    /// <typeparam name="TDestination"></typeparam>
    /// <param name="source"></param>
    /// <returns></returns>
    public static TDestination ProjectedAs<TSource, TDestination>(TSource source)
        where TSource : class
        where TDestination : class
    {
        var adapter = TypeAdapterFactory.CreateAdapter();
        return adapter.Adapt<TSource, TDestination>(source);
    }

    /// <summary>
    /// Project <typeparamref name="TSource"/> to a exists instance of type <typeparamref name="TDestination"/>
    /// </summary>
    /// <typeparam name="TSource">The source </typeparam>
    /// <typeparam name="TDestination"></typeparam>
    /// <param name="source"></param>
    /// <param name="destination"></param>
    /// <returns></returns>
    public static TDestination ProjectedAs<TSource, TDestination>(TSource source, TDestination destination)
        where TSource : class
        where TDestination : class
    {
        var adapter = TypeAdapterFactory.CreateAdapter();
        return adapter.Adapt(source, destination);
    }

    /// <summary>
    /// Project object to <typeparamref name="TDestination"/>
    /// </summary>
    /// <typeparam name="TDestination"></typeparam>
    /// <param name="item"></param>
    /// <returns></returns>
    public static TDestination ProjectedAs<TDestination>(object item)
        where TDestination : class
    {
        var adapter = TypeAdapterFactory.CreateAdapter();
        return adapter.Adapt<TDestination>(item);
    }
}