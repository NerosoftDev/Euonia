namespace Nerosoft.Euonia.Modularity;

/// <summary>
/// The service registration action.
/// </summary>
public class ServiceRegistrationAction : List<Action<IServiceRegistrationContext>>
{
    /// <summary>
    /// Gets or sets a value indicating whether the class interceptors are disabled.
    /// </summary>
    public bool IsClassInterceptorsDisabled { get; set; }
}