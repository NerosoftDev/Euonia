namespace Nerosoft.Euonia.Domain;

/// <summary>
/// Represent the object has an UpdateTime property.
/// </summary>
public interface IHasUpdateTime
{
    /// <summary>
    /// Gets or sets the last modify time of the object.
    /// </summary>
    DateTime UpdateTime { get; set; }
}