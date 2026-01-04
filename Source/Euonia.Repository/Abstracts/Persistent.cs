namespace Nerosoft.Euonia.Repository;

/// <summary>
/// Represents the base class for persistent entities with a typed identifier.
/// </summary>
/// <typeparam name="TKey"></typeparam>
public abstract class Persistent<TKey> : Persistent, IPersistent<TKey>
	where TKey : IEquatable<TKey>
{
	/// <summary>
	/// Get or set the entity identifier.
	/// </summary>
	public virtual TKey Id { get; set; }

	/// <inheritdoc/>
	public override object[] GetKeys()
	{
		return [Id];
	}

	/// <inheritdoc />
	public override string ToString()
	{
		return $"[ENTITY: {GetType().Name}] Id = {Id}";
	}
}

/// <summary>
/// Represents the base class for persistent entities.
/// </summary>
public abstract class Persistent : IPersistent
{
	/// <inheritdoc />
	public abstract object[] GetKeys();

	/// <inheritdoc/>
	public override string ToString()
	{
		return $"[ENTITY: {GetType().Name}] Keys = {string.Join(", ", GetKeys())}";
	}
}