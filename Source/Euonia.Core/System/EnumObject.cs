namespace System;

/// <summary>
/// Represents an enum object with a name and value.
/// </summary>
/// <typeparam name="TValue"></typeparam>
public class EnumObject<TValue>
{
    /// <summary>
    /// Gets or sets the name of the enum object.
    /// </summary>
    public string Name { get; set; } = default!;

    /// <summary>
    /// Gets or sets the value of the enum object.
    /// </summary>
    public TValue Value { get; set; } = default!;
}