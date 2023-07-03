namespace Nerosoft.Euonia.Modularity;

public class ApplicationInitializationContext : IServiceProviderAccessor
{
    public ApplicationInitializationContext(IServiceProvider serviceProvider)
    {
        ServiceProvider = serviceProvider;
    }

    public IServiceProvider ServiceProvider { get; set; }
}
