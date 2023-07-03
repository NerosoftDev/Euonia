namespace Nerosoft.Euonia.Modularity;

public class ServiceRegistrationAction : List<Action<IServiceRegistrationContext>>
{
    public bool IsClassInterceptorsDisabled { get; set; }
}