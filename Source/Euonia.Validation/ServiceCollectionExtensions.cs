using Nerosoft.Euonia.Validation;

// ReSharper disable MemberCanBePrivate.Global

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extension methods for setting up object validation services in an <see cref="T:Microsoft.Extensions.DependencyInjection.IServiceCollection" /> instance.
/// </summary>
public static class ServiceCollectionExtensions
{
	/// <summary>
	/// Adds object validator to the specified <see cref="T:Microsoft.Extensions.DependencyInjection.IServiceCollection" />.
	/// </summary>
	/// <param name="services"></param>
	/// <param name="types"></param>
	public static void AddValidator(this IServiceCollection services, IEnumerable<Type> types)
	{
		try
		{
			if (types == null || !types.Any())
			{
				return;
			}

			var validatorImplements = types.Where(type => typeof(FluentValidation.IValidator).IsAssignableFrom(type) && type.IsClass && !type.IsAbstract).ToList();

			foreach (var validatorType in validatorImplements)
			{
				var inheritedType = validatorType.GetInterfaces().FirstOrDefault(t => t.IsGenericType);
				if (inheritedType == null)
				{
					continue;
				}

				if (inheritedType.GenericTypeArguments.Length != 1)
				{
					continue;
				}

				var objectType = inheritedType.GenericTypeArguments[0];
				if (!objectType.IsClass || objectType.IsAbstract || objectType.IsEnum)
				{
					continue;
				}

				var interfaceType = typeof(FluentValidation.IValidator<>).MakeGenericType(objectType);
				services.AddSingleton(interfaceType, validatorType);
			}

			services.AddSingleton<IValidatorFactory, DefaultValidatorFactory>();
		}
		catch (Exception exception)
		{
			Console.WriteLine(exception);
			throw;
		}
	}

	/// <summary>
	/// Adds object validator to the specified <see cref="T:Microsoft.Extensions.DependencyInjection.IServiceCollection" />.
	/// </summary>
	/// <param name="services"></param>
	/// <param name="factory"></param>
	public static void AddValidator(this IServiceCollection services, Func<IEnumerable<Type>> factory)
	{
		var types = factory?.Invoke();
		if (types == null || !types.Any())
		{
			return;
		}

		services.AddValidator(types);
	}
}