using Microsoft.Extensions.DependencyInjection;
using Nerosoft.Euonia.Modularity;

namespace Nerosoft.Euonia.Validation;

public class ValidationModule : ModuleContextBase
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddAutomaticRegistrar(new ValidationRegistrar());
        context.Services.AddSingleton<IValidatorFactory, DefaultValidatorFactory>();
    }
}