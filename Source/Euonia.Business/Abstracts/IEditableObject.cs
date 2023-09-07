namespace Nerosoft.Euonia.Business;

/// <summary>
/// Represents an object which can be edited.
/// </summary>
public interface IEditableObject
{
    /// <summary>
    /// Gets the current object state.
    /// </summary>
    ObjectEditState State { get; }

    /// <summary>
    /// Gets a value indicate if the object can save.
    /// </summary>
    bool IsSavable { get; }

    /// <summary>
    /// Gets a value indicate whether the object is new.
    /// </summary>
    bool IsInsert { get; }

    /// <summary>
    /// Gets a value indicate whether the object is modified.
    /// </summary>
    bool IsUpdate { get; }

    /// <summary>
    /// Gets a value indicate whether the object is deleted.
    /// </summary>
    bool IsDelete { get; }

    /// <summary>
    /// Marks the object as insert.
    /// </summary>
    void MarkAsInsert();

    /// <summary>
    /// Marks the object as updated.
    /// </summary>
    void MarkAsUpdate();

    /// <summary>
    /// Marks the object as deleted.
    /// </summary>
    void MarkAsDelete();
}