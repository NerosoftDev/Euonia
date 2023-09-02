namespace Nerosoft.Euonia.Business;

/// <summary>
/// Metadata about a business object property.
/// </summary>
public interface IPropertyInfo : IMemberInfo, IComparable
{
    /// <summary>
    /// Gets the type of the property.
    /// </summary>
    Type Type { get; }

    /// <summary>
    /// Gets the default initial value for the property.
    /// </summary>
    object DefaultValue { get; }

    /// <summary>
    /// Gets a new <see cref="IFieldData"/> with specified name.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    IFieldData NewFieldData(string name);
    
    /// <summary>
    /// Gets the System.Reflection.PropertyInfo object
    /// </summary>
    /// <returns></returns>
    System.Reflection.PropertyInfo GetPropertyInfo();
}