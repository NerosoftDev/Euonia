namespace Nerosoft.Euonia.Domain;

/// <summary>
/// Represent the object has a CreateTime property.
/// </summary>
public interface IHasCreateTime
{
    /// <summary>
    /// Gets or sets the create time.
    /// </summary>
    DateTime CreateTime { get; set; }
}