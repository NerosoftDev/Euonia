namespace Nerosoft.Euonia.Modularity;

public interface IApplicationWithServiceProvider: IModularityApplication
{
    void SetServiceProvider(IServiceProvider serviceProvider);
    
    void Initialize(IServiceProvider serviceProvider);
}