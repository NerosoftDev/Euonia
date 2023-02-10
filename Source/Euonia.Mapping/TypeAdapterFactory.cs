namespace Nerosoft.Euonia.Mapping;

/// <summary>
/// The type adapter factory.
/// </summary>
public class TypeAdapterFactory
{
    #region Members

    private static ITypeAdapterFactory _factory;

    #endregion

    #region Public Static Methods

    /// <summary>
    /// Set the current type adapter factory
    /// </summary>
    /// <param name="adapterFactory">The adapter factory to set</param>
    public static void SetCurrent(ITypeAdapterFactory adapterFactory)
    {
        _factory = adapterFactory;
    }

    /// <summary>
    /// Create a new type adapter from current factory
    /// </summary>
    /// <returns>Created type adapter</returns>
    public static ITypeAdapter CreateAdapter()
    {
        return _factory?.Create();
    }

    #endregion
}