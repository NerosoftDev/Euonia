namespace Nerosoft.Euonia.Modularity;

/// <summary>
/// The context of the application shutdown logic.
/// </summary>
public class ApplicationShutdownContext
{
	/// <summary>
	/// Gets the <see cref="IServiceProvider"/> that can be used to resolve services from the dependency injection container.
	/// </summary>
    public IServiceProvider ServiceProvider { get; }

	/// <summary>
	/// Initializes a new instance of the <see cref="ApplicationShutdownContext"/> class.
	/// </summary>
	/// <param name="serviceProvider"></param>
    public ApplicationShutdownContext(IServiceProvider serviceProvider)
    {
        Check.EnsureNotNull(serviceProvider, nameof(serviceProvider));

        ServiceProvider = serviceProvider;
    }
}
