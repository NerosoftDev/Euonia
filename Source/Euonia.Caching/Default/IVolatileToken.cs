namespace Nerosoft.Euonia.Caching;

/// <summary>
/// Interface IVolatileToken
/// </summary>
public interface IVolatileToken
{
    /// <summary>
    /// Gets a value indicating whether this instance is current.
    /// </summary>
    /// <value><c>true</c> if this instance is current; otherwise, <c>false</c>.</value>
    bool IsCurrent { get; }
}