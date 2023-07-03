namespace Nerosoft.Euonia.Domain;

public static class AggregateExtensions
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="entity"></param>
    /// <typeparam name="TKey"></typeparam>
    public static void AttachToEvents<TKey>(this Aggregate<TKey> entity)
        where TKey : IEquatable<TKey>
    {
        foreach (var @event in entity.GetEvents())
        {
            @event.Attach(entity);
        }
    }

    // ReSharper disable once ParameterTypeCanBeEnumerable.Global
    /// <summary>
    /// 
    /// </summary>
    /// <param name="source"></param>
    /// <param name="id"></param>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    /// <returns></returns>
    public static TEntity Find<TEntity, TKey>(this HashSet<TEntity> source, TKey id)
        where TEntity : IEntity<TKey>
        where TKey : IEquatable<TKey>
    {
        return source.FirstOrDefault(t => t.Id.Equals(id));
    }
}