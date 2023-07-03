using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Nerosoft.Euonia.Modularity;

public interface IAutomaticRegistration
{
    void AddAssembly(IServiceCollection services, Assembly assembly);

    void AddTypes(IServiceCollection services, params Type[] types);

    void AddType(IServiceCollection services, Type type);
}
