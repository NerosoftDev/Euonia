namespace Nerosoft.Euonia.Caching;

/// <summary>
/// Class CacheKey.
/// Implements the <see cref="System.Tuple{Type, Type, Type}" />
/// </summary>
/// <seealso cref="System.Tuple{Type, Type, Type}" />
internal class CacheKey : Tuple<Type, Type, Type>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CacheKey"/> class.
    /// </summary>
    /// <param name="component">The component.</param>
    /// <param name="key">The key.</param>
    /// <param name="result">The result.</param>
    public CacheKey(Type component, Type key, Type result)
        : base(component, key, result)
    {
    }
}