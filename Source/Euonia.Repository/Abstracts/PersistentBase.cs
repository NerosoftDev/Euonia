namespace Nerosoft.Euonia.Repository;

/// <summary>
/// Base abstract class for persistent entities identified by a key of type <typeparamref name="TKey"/>.
/// </summary>
/// <typeparam name="TKey">The type of the primary key. Must implement <see cref="IEquatable{TKey}"/>.</typeparam>
public abstract class PersistentBase<TKey> : IPersistent<TKey>
	where TKey : IEquatable<TKey>
{
	/// <summary>
	/// Returns the primary key(s) of the entity as an array of objects.
	/// </summary>
	/// <remarks>
	/// The method currently returns a single-element array containing <see cref="Id"/>.
	/// Ensure the returned structure matches repository expectations.
	/// </remarks>
	/// <returns>An array of objects representing the entity key(s).</returns>
	public object[] GetKeys()
	{
		return [Id!];
	}

	/// <summary>
	/// Gets or sets the identifier for the entity.
	/// </summary>
	public TKey Id { get; set; }
}