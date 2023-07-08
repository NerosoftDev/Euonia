using Microsoft.Extensions.DependencyInjection;
using Nerosoft.Euonia.Modularity;

namespace Nerosoft.Euonia.Validation;

/// <inheritdoc />
public class ValidationModule : ModuleContextBase
{
    /// <inheritdoc />
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddAutomaticRegistrar(new ValidationRegistrar());
        context.Services.AddSingleton<IValidatorFactory, DefaultValidatorFactory>();
    }
}