// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedType.Global

namespace System;

/// <summary>
/// Indicate that the property or parameter would resolve from service container.
/// </summary>
[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property)]
public class InjectAttribute : Attribute
{
    /// <summary>
    /// Initialize a new instance of <see cref="InjectAttribute"/>.
    /// </summary>
    /// <param name="key">An object that specifies the key of service object to get.</param>
    public InjectAttribute(object key = null)
    {
        Key = key;
    }

    /// <summary>
    /// An object that specifies the key of service object to get.
    /// </summary>
    public object Key { get; }
}