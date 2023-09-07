namespace Nerosoft.Euonia.Modularity;

/// <summary>
/// The context of the application initialization.
/// </summary>
public class ApplicationInitializationContext : IServiceProviderAccessor
{
	/// <summary>
	/// Initializes a new instance of the <see cref="ApplicationInitializationContext"/> class.
	/// </summary>
	/// <param name="serviceProvider"></param>
    public ApplicationInitializationContext(IServiceProvider serviceProvider)
    {
        ServiceProvider = serviceProvider;
    }

	/// <summary>
	/// Gets or sets the <see cref="IServiceProvider"/> instance.
	/// </summary>
    public IServiceProvider ServiceProvider { get; set; }
}
