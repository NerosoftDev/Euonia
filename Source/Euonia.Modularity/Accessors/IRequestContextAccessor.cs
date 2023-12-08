namespace Nerosoft.Euonia.Modularity;

/// <summary>
/// Defines the interface of a request context accessor.
/// </summary>
public interface IRequestContextAccessor : IScopedDependency
{
	/// <summary>
	/// Gets the current request context instance.
	/// </summary>
	RequestContext Context { get; }
}
