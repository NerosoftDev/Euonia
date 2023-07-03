namespace Nerosoft.Euonia.Domain;

/// <summary>
/// Specifies the contract for aggregate root.
/// </summary>
public interface IAggregateRoot : IEntity
{
}

/// <summary>
/// Specifies the contract for aggregate root.
/// </summary>
/// <typeparam name="TKey">The identifier type.</typeparam>
public interface IAggregateRoot<TKey> : IAggregateRoot, IEntity<TKey>
    where TKey : IEquatable<TKey>
{
}