namespace Nerosoft.Euonia.Mapping;

public class AutomapperTypeAdapterFactory : ITypeAdapterFactory
{
    private readonly IServiceProvider _serviceProvider;

    public AutomapperTypeAdapterFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public ITypeAdapter Create()
    {
        return new AutomapperTypeAdapter(_serviceProvider);
    }
}