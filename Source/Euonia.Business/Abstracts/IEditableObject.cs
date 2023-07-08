namespace Nerosoft.Euonia.Business;

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

    bool IsInsert { get; }

    bool IsUpdate { get; }

    bool IsDelete { get; }

    void MarkAsInsert();

    void MarkAsUpdate();

    void MarkAsDelete();
}