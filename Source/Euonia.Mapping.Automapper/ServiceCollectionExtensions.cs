using AutoMapper;
using Google.Protobuf.Collections;
using Google.Protobuf.WellKnownTypes;
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
		try
		{
			var expression = new MapperConfigurationExpression();

			expression.MapGrpcTypes();

			return services.AddSingleton(provider =>
			{
				var options = provider.GetService<IOptions<AutomapperOptions>>()?.Value;
				if (options != null)
				{
					foreach (var configurator in options.Configurators)
					{
						configurator(expression);
					}
				}

				var mapperConfiguration = new MapperConfiguration(expression);

				foreach (var profileType in (options?.ValidatingProfiles ?? new TypeList<Profile>()))
				{
					var profile = (Profile)Activator.CreateInstance(profileType);
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
		catch (Exception exception)
		{
			Console.WriteLine(exception);
			throw;
		}
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
		try
		{
			var expression = new MapperConfigurationExpression();

			expression.MapGrpcTypes();

			if (types != null)
			{
				foreach (var type in types)
				{
					expression.AddProfile(type);
				}
			}

			config?.Invoke(expression);
			var mapperConfiguration = new MapperConfiguration(expression);

			if (assertConfiguration)
			{
				mapperConfiguration.AssertConfigurationIsValid();
			}

			var mapper = mapperConfiguration.CreateMapper();

			return services.AddSingleton(mapper);
		}
		catch (Exception exception)
		{
			Console.WriteLine(exception);
			throw;
		}
	}

	/// <summary>
	/// Adds <see cref="Mapper"/> as object mapping provider.
	/// </summary>
	/// <param name="services"></param>
	/// <param name="config"></param>
	/// <param name="configureGrpcTypeMapping"></param>
	/// <param name="assertConfiguration"></param>
	/// <returns>A reference to this instance after the operation has completed.</returns>
	public static IServiceCollection AddAutomapper(this IServiceCollection services, Action<MapperConfigurationExpression> config, bool configureGrpcTypeMapping = false, bool assertConfiguration = false)
	{
		try
		{
			var expression = new MapperConfigurationExpression();

			if (configureGrpcTypeMapping)
			{
				expression.MapGrpcTypes();
			}

			config?.Invoke(expression);
			var mapperConfiguration = new MapperConfiguration(expression);

			if (assertConfiguration)
			{
				mapperConfiguration.AssertConfigurationIsValid();
			}

			var mapper = mapperConfiguration.CreateMapper();

			return services.AddSingleton(mapper);
		}
		catch (Exception exception)
		{
			Console.WriteLine(exception);
			throw;
		}
	}

	private static void MapGrpcTypes(this IProfileExpression expression)
	{
		/*
		expression.ForAllPropertyMaps(map => map.SourceType == typeof(Timestamp), (_, options) =>
		{
		    options.ConvertUsing(typeof(TimestampToDatetimeValueConverter));
		});
		expression.ForAllPropertyMaps(map => map.DestinationType == typeof(Timestamp), (_, options) =>
		{
		    options.ConvertUsing<DatetimeToTimestampValueConverter, object>();
		});
		expression.ForAllPropertyMaps(map => map.SourceType == typeof(Duration), (_, options) =>
		{
		    options.ConvertUsing(typeof(DurationToTimespanValueConverter));
		});
		expression.ForAllPropertyMaps(map => map.DestinationType == typeof(Duration), (_, options) =>
		{
		    options.ConvertUsing<TimespanToDurationValueConverter, object>();
		});
		*/

		expression.CreateMap<Timestamp, DateTime?>()
		          .ConvertUsing<TimestampToNullableDatetimeTypeConverter>();
		expression.CreateMap<Timestamp, DateTime>()
		          .ConvertUsing<TimestampToNotnullDatetimeTypeConverter>();
		expression.CreateMap<DateTime?, Timestamp>()
		          .ConvertUsing<DatetimeToTimestampTypeConverter<DateTime?>>();
		expression.CreateMap<DateTime, Timestamp>()
		          .ConvertUsing<DatetimeToTimestampTypeConverter<DateTime>>();

		expression.CreateMap<TimeSpan?, Duration>()
		          .ConvertUsing<TimespanToDurationTypeConverter<TimeSpan?>>();
		expression.CreateMap<TimeSpan, Duration>()
		          .ConvertUsing<TimespanToDurationTypeConverter<TimeSpan>>();
		expression.CreateMap<Duration, TimeSpan?>()
		          .ConvertUsing<DurationToNullableTimespanTypeConverter>();
		expression.CreateMap<Duration, TimeSpan>()
		          .ConvertUsing<DurationToNotnullTimespanTypeConverter>();

		expression.CreateMap(typeof(IEnumerable<>), typeof(RepeatedField<>))
		          .ConvertUsing(typeof(ListToRepeatedFieldTypeConverter<,>));
		expression.CreateMap(typeof(RepeatedField<>), typeof(IEnumerable<>))
		          .ConvertUsing(typeof(RepeatedFieldToListTypeConverter<,>));
	}
}