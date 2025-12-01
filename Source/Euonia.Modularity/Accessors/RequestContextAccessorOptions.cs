namespace Nerosoft.Euonia.Modularity;

/// <summary>
/// The options for <see cref="RequestContextAccessor"/>.
/// </summary>
public class RequestContextAccessorOptions
{
	/// <summary>
    /// Gets or sets a value indicating whether to use the default request context accessor or the delegate accessor.
    /// </summary>
	public bool UseDefaultAccessor { get; set; } = true;
}