using Microsoft.Extensions.DependencyInjection;
using Nerosoft.Euonia.Modularity;

// ReSharper disable UnusedType.Global

namespace Nerosoft.Euonia.Validation;

/// <summary>
/// Set depends on <see cref="ValidationModule"/> to import object validation services.
/// </summary>
public class ValidationModule : ModuleContextBase
{
	/// <inheritdoc />
	public override void ConfigureServices(ServiceConfigurationContext context)
	{
		context.Services.AddAutomaticRegistrar(new ValidationRegistrar());
		context.Services.AddSingleton<IValidatorFactory, DefaultValidatorFactory>();
	}
}