using AutoMapper;
using Microsoft.Extensions.Logging;
using Nerosoft.Euonia.Mapping;
using Type = System.Type;
using Microsoft.Extensions.Options;
using Nerosoft.Euonia.Collections;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// The <see cref="IServiceCollection"/> extensions to register object mapping provider.
/// </summary>
public static class ServiceCollectionExtensions
{
	/// <summary>
	/// Adds <see cref="Mapper"/> as object mapping provider.
	/// </summary>
	/// <param name="services"></param>
	/// <param name="factory"></param>
	/// <param name="config"></param>
	/// <param name="assertConfiguration"></param>
	/// <returns>A reference to this instance after the operation has completed.</returns>
	public static IServiceCollection AddAutomapper(this IServiceCollection services, Func<IEnumerable<Type>> factory, Action<MapperConfigurationExpression> config = null, bool assertConfiguration = false)
	{
		var types = factory?.Invoke();
		return services.AddAutomapper(types, config, assertConfiguration);
	}

	/// <summary>
	/// Adds &lt;see cref="Mapper"/&gt; as object mapping provider.
	/// </summary>
	/// <param name="services"></param>
	/// <param name="handler"></param>
	/// <param name="config"></param>
	/// <param name="assertConfiguration"></param>
	/// <returns>A reference to this instance after the operation has completed.</returns>
	public static IServiceCollection AddAutomapper(this IServiceCollection services, Action<List<Type>> handler, Action<MapperConfigurationExpression> config = null, bool assertConfiguration = false)
	{
		var types = new List<Type>();
		handler?.Invoke(types);
		return services.AddAutomapper(types, config, assertConfiguration);
	}

	/// <summary>
	/// Adds <see cref="Mapper"/> as object mapping provider.
	/// </summary>
	/// <param name="services"></param>
	/// <exception cref="Exception"></exception>
	/// <returns>A reference to this instance after the operation has completed.</returns>
	public static IServiceCollection AddAutomapper(this IServiceCollection services)
	{
		

		return services.AddSingleton(provider =>
		{
			var logger = provider.GetService<ILoggerFactory>();
			
			var expression = new MapperConfigurationExpression();
			expression.ConstructServicesUsing(type => ActivatorUtilities.GetServiceOrCreateInstance(provider, type));
			var options = provider.GetService<IOptions<AutomapperOptions>>()?.Value;
			if (options != null)
			{
				foreach (var configurator in options.Configurators)
				{
					configurator(provider, expression);
				}
			}

			var mapperConfiguration = new MapperConfiguration(expression, logger);

			foreach (var profileType in (options?.ValidatingProfiles ?? new TypeList<Profile>()))
			{
				var profile = (Profile)ActivatorUtilities.CreateInstance(provider, profileType);
				if (profile == null)
				{
					throw new Exception($"{profileType} is a not valid AutoMapper profile.");
				}

				mapperConfiguration.AssertConfigurationIsValid();
				//mapperConfiguration.AssertConfigurationIsValid(profile.ProfileName);
			}

			var mapper = mapperConfiguration.CreateMapper();
			return mapper;
		});
	}

	/// <summary>
	/// Adds <see cref="Mapper"/> as object mapping provider.
	/// </summary>
	/// <param name="services"></param>
	/// <param name="types"></param>
	/// <param name="config"></param>
	/// <param name="assertConfiguration"></param>
	/// <returns>A reference to this instance after the operation has completed.</returns>
	public static IServiceCollection AddAutomapper(this IServiceCollection services, IEnumerable<Type> types, Action<MapperConfigurationExpression> config = null, bool assertConfiguration = false)
	{
		return services.AddSingleton(provider =>
		{
			var logger = provider.GetService<ILoggerFactory>();

			var expression = new MapperConfigurationExpression();

			if (types != null)
			{
				foreach (var type in types)
				{
					expression.AddProfile(type);
				}
			}

			config?.Invoke(expression);
			var mapperConfiguration = new MapperConfiguration(expression, logger);

			if (assertConfiguration)
			{
				mapperConfiguration.AssertConfigurationIsValid();
			}

			var mapper = mapperConfiguration.CreateMapper();
			return mapper;
		});
	}

	/// <summary>
	/// Adds <see cref="Mapper"/> as object mapping provider.
	/// </summary>
	/// <param name="services"></param>
	/// <param name="config"></param>
	/// <param name="assertConfiguration"></param>
	/// <returns>A reference to this instance after the operation has completed.</returns>
	public static IServiceCollection AddAutomapper(this IServiceCollection services, Action<MapperConfigurationExpression> config, bool assertConfiguration = false)
	{
		return services.AddSingleton(provider =>
		{
			var logger = provider.GetService<ILoggerFactory>();

			var expression = new MapperConfigurationExpression();

			config?.Invoke(expression);
			var mapperConfiguration = new MapperConfiguration(expression, logger);

			if (assertConfiguration)
			{
				mapperConfiguration.AssertConfigurationIsValid();
			}

			var mapper = mapperConfiguration.CreateMapper();
			return mapper;
		});
	}
}