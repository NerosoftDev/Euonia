using System.Diagnostics.CodeAnalysis;
using Quartz;
using Quartz.Spi;

namespace Nerosoft.Euonia.Quartz;

/// <summary>
/// The background build options.
/// </summary>
[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
[SuppressMessage("ReSharper", "UnusedMember.Global")]
public class BackgroundBuildOptions
{
	/// <summary>
	/// Gets the jobs.
	/// </summary>
	internal Dictionary<Type, BackgroundJobOptions> Jobs { get; } = new();

	/// <summary>
	/// The actions to configure the <see cref="IServiceCollectionQuartzConfigurator"/>.
	/// </summary>
	internal List<Action<IServiceCollectionQuartzConfigurator>> Configurations { get; } = new();

	/// <summary>
	/// Gets or sets the scheduler identifier.
	/// </summary>
	public string SchedulerId { get; set; }

	/// <summary>
	/// Gets or sets the scheduler name.
	/// </summary>
	public string SchedulerName { get; set; }

	/// <summary>
	/// Gets or sets the misfire threshold time.
	/// </summary>
	public TimeSpan? MisfireThreshold { get; set; }

	/// <summary>
	/// 
	/// </summary>
	public bool IgnoreDuplicates { get; set; } = true;

	/// <summary>
	/// Gets or sets a value indicating whether the existing scheduling data (with same identifiers) will be overwritten.
	/// </summary>
	public bool OverWriteExistingData { get; set; } = true;

	/// <summary>
	/// 
	/// </summary>
	public bool? InterruptJobsOnShutdown { get; set; }

	/// <summary>
	/// 
	/// </summary>
	public bool? InterruptJobsOnShutdownWithWait { get; set; }

	/// <summary>
	/// 
	/// </summary>
	public int? MaxBatchSize { get; set; }

	/// <summary>
	/// Gets or sets the type loader.
	/// </summary>
	public Type TypeLoader { get; set; }

	/// <summary>
	/// If <see langword="true" /> the scheduler will not allow shutdown process
	/// to return until all currently executing jobs have completed.
	/// </summary>
	public bool WaitForJobsToComplete { get; set; }

	/// <summary>
	/// <para>
	/// If not <see langword="null" /> the scheduler will start after specified delay.
	/// </para>
	/// <para>
	/// If <see cref="AwaitApplicationStarted"/> is true, the delay starts when application startup completes.
	/// </para>
	/// </summary>
	public TimeSpan? StartDelay { get; set; }

	/// <summary>
	/// If true (default), jobs will not be started until application startup completes.
	/// This avoids the running of jobs <em>during</em> application startup.
	/// </summary>
	public bool AwaitApplicationStarted { get; set; } = true;

	/// <summary>
	/// Adds the job.
	/// </summary>
	/// <typeparam name="TJob"></typeparam>
	public void AddJob<TJob>()
		where TJob : IJob
	{
		AddJob<TJob>(options: null);
	}

	/// <summary>
	/// Adds the job.
	/// </summary>
	/// <param name="options"></param>
	/// <typeparam name="TJob"></typeparam>
	public void AddJob<TJob>(BackgroundJobOptions options)
		where TJob : IJob
	{
		Jobs[typeof(TJob)] = options;
	}

	/// <summary>
	/// Adds the job.
	/// </summary>
	/// <param name="action"></param>
	/// <typeparam name="TJob"></typeparam>
	public void AddJob<TJob>(Action<BackgroundJobOptions> action)
		where TJob : IJob
	{
		var options = new BackgroundJobOptions();
		action?.Invoke(options);
		AddJob<TJob>(options);
	}

	/// <summary>
	/// Adds the job.
	/// </summary>
	/// <param name="jobType"></param>
	public void AddJob(Type jobType)
	{
		AddJob(jobType, options: null);
	}

	/// <summary>
	/// Adds the job.
	/// </summary>
	/// <param name="jobType"></param>
	/// <param name="options"></param>
	public void AddJob(Type jobType, BackgroundJobOptions options)
	{
		Jobs[jobType] = options;
	}

	/// <summary>
	/// Adds the job.
	/// </summary>
	public void AddJob(Type jobType, Action<BackgroundJobOptions> action)
	{
		var options = new BackgroundJobOptions();
		action?.Invoke(options);
		AddJob(jobType, options);
	}

	/// <summary>
	/// Configures the scheduler.
	/// </summary>
	/// <param name="action"></param>
	/// <returns></returns>
	public BackgroundBuildOptions Configure(Action<IServiceCollectionQuartzConfigurator> action)
	{
		Configurations.Add(action);
		return this;
	}

	/// <summary>
	/// Use simple type loader.
	/// </summary>
	/// <returns></returns>
	public BackgroundBuildOptions UseSimpleTypeLoader()
	{
		Configurations.Add(cfg => cfg.UseSimpleTypeLoader());
		return this;
	}

	/// <summary>
	/// Use type loader of <typeparamref name="T"/>.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <returns></returns>
	public BackgroundBuildOptions UseTypeLoader<T>()
		where T : ITypeLoadHelper
	{
		Configurations.Add(cfg => cfg.UseTypeLoader<T>());
		return this;
	}

	/// <summary>
	/// Use in-memory store.
	/// </summary>
	/// <param name="configure"></param>
	/// <returns></returns>
	public BackgroundBuildOptions UseInMemoryStore(Action<SchedulerBuilder.InMemoryStoreOptions> configure = null)
	{
		Configurations.Add(cfg => cfg.UseInMemoryStore(configure));
		return this;
	}

	/// <summary>
	/// Use persistent store.
	/// </summary>
	/// <param name="configure"></param>
	/// <returns></returns>
	public BackgroundBuildOptions UsePersistentStore(Action<SchedulerBuilder.PersistentStoreOptions> configure)
	{
		Configurations.Add(cfg => cfg.UsePersistentStore(configure));
		return this;
	}

	/// <summary>
	/// Use persistent store of <typeparamref name="T"/>.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="configure"></param>
	/// <returns></returns>
	public BackgroundBuildOptions UsePersistentStore<T>(Action<SchedulerBuilder.PersistentStoreOptions> configure)
		where T : class, IJobStore
	{
		Configurations.Add(cfg => cfg.UsePersistentStore<T>(configure));
		return this;
	}

	/// <summary>
	/// Use thread pool of <typeparamref name="T"/>.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="configure"></param>
	/// <returns></returns>
	public BackgroundBuildOptions UseThreadPool<T>(Action<SchedulerBuilder.ThreadPoolOptions> configure = null)
		where T : class, IThreadPool
	{
		Configurations.Add(cfg => cfg.UseThreadPool<T>(configure));
		return this;
	}

	/// <summary>
	/// Use default thread pool.
	/// </summary>
	/// <param name="maxConcurrency"></param>
	/// <param name="configure"></param>
	/// <returns></returns>
	public BackgroundBuildOptions UseDefaultThreadPool(int maxConcurrency, Action<SchedulerBuilder.ThreadPoolOptions> configure = null)
	{
		Configurations.Add(cfg => cfg.UseDefaultThreadPool(maxConcurrency, configure));
		return this;
	}

	/// <summary>
	/// Use default thread pool.
	/// </summary>
	/// <param name="configure"></param>
	/// <returns></returns>
	public BackgroundBuildOptions UseDefaultThreadPool(Action<SchedulerBuilder.ThreadPoolOptions> configure = null)
	{
		Configurations.Add(cfg => cfg.UseDefaultThreadPool(configure));
		return this;
	}

	/// <summary>
	/// Use zero size thread pool.
	/// </summary>
	/// <param name="configure"></param>
	/// <returns></returns>
	public BackgroundBuildOptions UseZeroSizeThreadPool(Action<SchedulerBuilder.ThreadPoolOptions> configure = null)
	{
		Configurations.Add(cfg => cfg.UseZeroSizeThreadPool(configure));
		return this;
	}

	/// <summary>
	/// Use dedicated thread pool.
	/// </summary>
	/// <param name="maxConcurrency"></param>
	/// <param name="configure"></param>
	/// <returns></returns>
	public BackgroundBuildOptions UseDedicatedThreadPool(int maxConcurrency, Action<SchedulerBuilder.ThreadPoolOptions> configure = null)
	{
		Configurations.Add(cfg => cfg.UseDedicatedThreadPool(maxConcurrency, configure));
		return this;
	}

	/// <summary>
	/// Use dedicated thread pool.
	/// </summary>
	/// <param name="configure"></param>
	/// <returns></returns>
	public BackgroundBuildOptions UseDedicatedThreadPool(Action<SchedulerBuilder.ThreadPoolOptions> configure = null)
	{
		Configurations.Add(cfg => cfg.UseDedicatedThreadPool(configure));
		return this;
	}

	/// <summary>
	/// Set property.
	/// </summary>
	/// <param name="name"></param>
	/// <param name="value"></param>
	/// <returns></returns>
	public BackgroundBuildOptions SetProperty(string name, string value)
	{
		Configurations.Add(cfg => cfg.SetProperty(name, value));
		return this;
	}
}