namespace Nerosoft.Euonia.Bus.InMemory;

/// <summary>
/// A base interface masking <see cref="ITypeDictionary{TKey,TValue}"/> instances and exposing non-generic functionalities.
/// </summary>
internal interface ITypeDictionary
{
	/// <summary>
	/// Gets the count of entries in the dictionary.
	/// </summary>
	int Count { get; }

	/// <summary>
	/// Clears the current dictionary.
	/// </summary>
	void Clear();
}

/// <summary>
/// An interface providing key type contravariant access to a <see cref="ITypeDictionary{TKey,TValue}"/> instance.
/// </summary>
/// <typeparam name="TKey">The contravariant type of keys in the dictionary.</typeparam>
internal interface ITypeDictionary<in TKey> : ITypeDictionary
	where TKey : IEquatable<TKey>
{
	/// <summary>
	/// Tries to remove a value with a specified key, if present.
	/// </summary>
	/// <param name="key">The key of the value to remove.</param>
	/// <returns>Whether or not the key was present.</returns>
	bool TryRemove(TKey key);
}

/// <summary>
/// An interface providing key type contravariant and value type covariant access
/// to a <see cref="ITypeDictionary{TKey,TValue}"/> instance.
/// </summary>
/// <typeparam name="TKey">The contravariant type of keys in the dictionary.</typeparam>
/// <typeparam name="TValue">The covariant type of values in the dictionary.</typeparam>
internal interface ITypeDictionary<in TKey, out TValue> : ITypeDictionary<TKey>
	where TKey : IEquatable<TKey>
	where TValue : class
{
	/// <summary>
	/// Gets the value with the specified key.
	/// </summary>
	/// <param name="key">The key to look for.</param>
	/// <returns>The returned value.</returns>
	/// <exception cref="ArgumentException">Thrown if the key wasn't present.</exception>
	TValue this[TKey key] { get; }
}