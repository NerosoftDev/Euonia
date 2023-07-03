namespace Nerosoft.Euonia.Mapping;

/// <summary>
/// Base contract for map dto to aggregate or aggregate to dto.
/// <remarks>
/// This is a  contract for work with "auto" mappers ( automapper,emitmapper,valueinjecter...)
/// or adhoc mappers
/// </remarks>
/// </summary>
public interface ITypeAdapter
{
    /// <summary>
    /// Adapt a source object to an instance of type <typeparamref name="TDestination"/>
    /// </summary>
    /// <typeparam name="TSource">Type of source item</typeparam>
    /// <typeparam name="TDestination">Type of dest item</typeparam>
    /// <param name="source">Instance to adapt</param>
    /// <returns><paramref name="source"/> mapped to <typeparamref name="TDestination"/></returns>
    TDestination Adapt<TSource, TDestination>(TSource source)
        where TDestination : class
        where TSource : class;

    /// <summary>
    /// Adapt a source object to an instance of type <typeparamref name="TDestination"/>
    /// </summary>
    /// <param name="source">Instance to adapt</param>
    /// <param name="destination">Instance of destination</param>
    /// <typeparam name="TSource">Type of source item</typeparam>
    /// <typeparam name="TDestination">Type of dest item</typeparam>
    /// <returns><paramref name="source"/> mapped to <typeparamref name="TDestination"/></returns>
    TDestination Adapt<TSource, TDestination>(TSource source, TDestination destination)
        where TDestination : class
        where TSource : class;

    /// <summary>
    /// Adapt a source object to an instance of type <typeparamref name="TDestination"/>
    /// </summary>
    /// <typeparam name="TDestination">Type of dest item</typeparam>
    /// <param name="source">Instance to adapt</param>
    /// <returns><paramref name="source"/> mapped to <typeparamref name="TDestination"/></returns>
    TDestination Adapt<TDestination>(object source)
        where TDestination : class;

    /// <summary>
    /// Adapt a source object to an instance of destination type.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="destinationType"></param>
    /// <returns></returns>
    object Adapt(object source, Type destinationType);

    /// <summary>
    /// Adapt a source object to an instance of type <typeparamref name="TDestination"/>
    /// </summary>
    /// <param name="source">Instance to adapt</param>
    /// <param name="destination">Instance of destination</param>
    /// <typeparam name="TDestination">Type of dest item</typeparam>
    /// <returns><paramref name="source"/> mapped to <typeparamref name="TDestination"/></returns>
    TDestination Adapt<TDestination>(object source, TDestination destination)
        where TDestination : class;
}