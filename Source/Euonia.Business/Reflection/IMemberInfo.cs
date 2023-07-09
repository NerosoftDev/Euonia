namespace Nerosoft.Euonia.Business;

/// <summary>
/// Metadata of a business object member, usually a method or property.
/// </summary>
public interface IMemberInfo
{
    /// <summary>
    /// Gets the member name value.
    /// </summary>
    string Name { get; }
}