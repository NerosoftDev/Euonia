using System.Reflection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Nerosoft.Euonia.Business;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static void AddBusinessObject(this IServiceCollection services, params Assembly[] assemblies)
    {
        services.TryAddScoped<BusinessContextAccessor>();
        services.TryAddScoped<BusinessContext>();
        services.TryAddScoped<IObjectFactory, BusinessObjectFactory>();

        if (assemblies?.Length > 0)
        {
            var types = assemblies.SelectMany(assembly => assembly.GetTypes().Where(t => t.IsClass && !t.IsAbstract && t.IsAssignableTo(typeof(IBusinessObject))));

            foreach (var type in types)
            {
                services.TryAddTransient(type);
            }
        }

        {
        }
    }
}