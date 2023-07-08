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

    IFieldData NewFieldData(string name);
    
    /// <summary>
    /// Gets the System.Reflection.PropertyInfo object
    /// </summary>
    /// <returns></returns>
    System.Reflection.PropertyInfo GetPropertyInfo();
}