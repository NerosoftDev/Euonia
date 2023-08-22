// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedType.Global

namespace System;

/// <summary>
/// Indicate that the property or parameter would resolved from service container.
/// </summary>
[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property)]
public class InjectAttribute : Attribute
{
    /// <summary>
    /// Initialize a new instance of <see cref="InjectAttribute"/>.
    /// </summary>
    /// <param name="name"></param>
    public InjectAttribute(string name = null)
    {
        Name = name;
    }

    /// <summary>
    /// Specified the service name or service type name.
    /// </summary>
    public string Name { get; }
}