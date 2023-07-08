namespace Nerosoft.Euonia.Business;

public interface IUseBusinessContext
{
    BusinessContext BusinessContext { get; set; }

    IServiceProvider GetServiceProvider();
}