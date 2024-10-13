namespace Nerosoft.Euonia.Domain;

/// <summary>
/// Represent the object can be deleted logically.
/// </summary>
public interface ITombstone
{
    /// <summary>
    /// Gets or sets a value indicate whether the entry is deleted softly or not.
    /// </summary>
    /// <value><c>true</c> if the entry is deleted, otherwise <c>false</c>.</value>
    bool IsDeleted { get; set; }

    /// <summary>
    /// Gets or sets the entry delete time.
    /// </summary>
    DateTime? DeleteTime { get; set; }
}