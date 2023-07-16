using Microsoft.Extensions.DependencyInjection;

namespace Nerosoft.Euonia.Modularity;

/// <summary>
/// The application creation options.
/// </summary>
public class ApplicationCreationOptions
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ApplicationCreationOptions"/> class.
    /// </summary>
    /// <param name="services"></param>
    public ApplicationCreationOptions(IServiceCollection services)
    {
        Services = services;
        Configuration = new ConfigurationBuilderOptions();
    }

    /// <summary>
    /// Gets the services.
    /// </summary>
    public IServiceCollection Services { get; }

    /// <summary>
    /// Gets the configuration.
    /// </summary>
    public ConfigurationBuilderOptions Configuration { get; }
}
