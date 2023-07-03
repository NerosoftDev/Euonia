namespace Nerosoft.Euonia.Modularity;

public class ApplicationShutdownContext
{
    public IServiceProvider ServiceProvider { get; }

    public ApplicationShutdownContext(IServiceProvider serviceProvider)
    {
        Check.EnsureNotNull(serviceProvider, nameof(serviceProvider));

        ServiceProvider = serviceProvider;
    }
}
