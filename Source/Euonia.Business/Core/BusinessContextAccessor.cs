namespace Nerosoft.Euonia.Business;

public class BusinessContextAccessor
{
    public BusinessContextAccessor(IServiceProvider provider)
    {
        ServiceProvider = provider;
    }

    internal IServiceProvider ServiceProvider { get; private set; }
}