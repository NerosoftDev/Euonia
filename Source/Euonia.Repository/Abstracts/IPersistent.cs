namespace Nerosoft.Euonia.Repository;

/// <summary>
/// Specifies the contract for entity.
/// </summary>
public interface IPersistent
{
    /// <summary>
    /// Returns an array of ordered keys for this entity.
    /// </summary>
    /// <returns></returns>
    object[] GetKeys();
}

/// <summary>
/// Specifies the contract for entity with key of type <typeparamref name="TKey"/> .
/// </summary>
/// <typeparam name="TKey">The identifier type.</typeparam>
public interface IPersistent<TKey> : IPersistent
    where TKey : IEquatable<TKey>
{
    /// <summary>
    /// Gets or sets the identifier.
    /// </summary>
    /// <value>The identifier.</value>
    TKey Id { get; set; }
}