namespace Nerosoft.Euonia.Modularity;

/// <summary>
/// To be added.
/// </summary>
public interface IApplicationWithServiceProvider: IModularityApplication
{
    /// <summary>
    /// To be added.
    /// </summary>
    /// <param name="serviceProvider"></param>
    void SetServiceProvider(IServiceProvider serviceProvider);
    
    /// <summary>
    /// To be added.
    /// </summary>
    /// <param name="serviceProvider"></param>
    void Initialize(IServiceProvider serviceProvider);
}