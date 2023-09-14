using Nerosoft.Euonia.Quartz;
using Nerosoft.Euonia.Reflection;
using Quartz;

// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedType.Global

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Extension methods for setting up Quartz services in an <see cref="T:Microsoft.Extensions.DependencyInjection.IServiceCollection" />.
/// </summary>
public static class ServiceCollectionExtensions
{
	/// <summary>
	/// Adds Quartz services to the specified <see cref="T:Microsoft.Extensions.DependencyInjection.IServiceCollection" />.
	/// </summary>
	/// <param name="services"></param>
	/// <param name="optionsAction"></param>
	/// <returns></returns>
	public static IServiceCollection AddQuartz(this IServiceCollection services, Action<BackgroundBuildOptions> optionsAction)
	{
		var buildOptions = Singleton<BackgroundBuildOptions>.Instance;
		optionsAction?.Invoke(buildOptions);
		// if you are using persistent job store, you might want to alter some options
		services.Configure<QuartzOptions>(options =>
		{
			options.Scheduling.IgnoreDuplicates = buildOptions.IgnoreDuplicates; // default: false
			options.Scheduling.OverWriteExistingData = buildOptions.OverWriteExistingData; // default: true
			options.MisfireThreshold = buildOptions.MisfireThreshold;
		});

		services.AddQuartz(quartz =>
		{
			quartz.SchedulerId = buildOptions.SchedulerId;
			quartz.SchedulerName = buildOptions.SchedulerName;
			if (buildOptions.TypeLoader != null)
			{
				Reflect.InvokeGenericMethod(quartz, nameof(quartz.UseTypeLoader), new[] { buildOptions.TypeLoader });
			}
			else
			{
				quartz.UseSimpleTypeLoader();
			}

			quartz.UseInMemoryStore();
			quartz.UseDefaultThreadPool(tp =>
			{
				tp.MaxConcurrency = 10;
			});

			foreach (var (type, config) in buildOptions.Jobs)
			{
				quartz.AddJob(type, config);
			}
		});

		return services;
	}
}