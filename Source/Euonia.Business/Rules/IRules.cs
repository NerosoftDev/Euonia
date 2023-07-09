namespace Nerosoft.Euonia.Business;

/// <summary>
/// Public interface for <see cref="IRules"/>
/// </summary>
public interface IRules
{
    /// <summary>
    /// Gets the target business object
    /// </summary>
    /// <value>The business object.</value>
    object Target { get; }
}