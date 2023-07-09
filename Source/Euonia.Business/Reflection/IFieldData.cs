namespace Nerosoft.Euonia.Business;

public interface IFieldData
{
    /// <summary>
    /// Gets the name of the field.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Gets or sets the field value.
    /// </summary>
    /// <value>The value of the field.</value>
    /// <returns>The value of the field.</returns>
    object Value { get; set; }

    /// <summary>
    /// Gets a value indicate if the value has changed.
    /// </summary>
    bool IsChanged { get; }

    /// <summary>
    /// Marks the field as unchanged.
    /// </summary>
    void MarkAsUnchanged();

    /// <summary>
    /// Restore value to previous one.
    /// </summary>
    void Undo();
}

public interface IFieldData<T> : IFieldData
{
    /// <summary>
    /// Gets or sets the field value.
    /// </summary>
    /// <value>The value of the field.</value>
    /// <returns>The value of the field.</returns>
    new T Value { get; set; }
}