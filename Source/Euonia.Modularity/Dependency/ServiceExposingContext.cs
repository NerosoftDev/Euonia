namespace Nerosoft.Euonia.Modularity;

/// <summary>
/// The service exposing context implementation.
/// </summary>
public class ServiceExposingContext : IServiceExposingContext
{
	/// <inheritdoc />
	public Type ImplementationType { get; }

	/// <inheritdoc />
	public List<Type> ExposedTypes { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ServiceExposingContext"/> class.
    /// </summary>
    /// <param name="implementationType"></param>
    /// <param name="exposedTypes"></param>
    public ServiceExposingContext(Type implementationType, List<Type> exposedTypes)
    {
        ImplementationType = Check.EnsureNotNull(implementationType, nameof(implementationType));
        ExposedTypes = Check.EnsureNotNull(exposedTypes, nameof(exposedTypes));
    }
}
