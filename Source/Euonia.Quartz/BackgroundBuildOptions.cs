using System.Diagnostics.CodeAnalysis;
using Quartz;

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
	/// 
	/// </summary>
	public bool OverWriteExistingData { get; set; } = true;

	/// <summary>
	/// Gets or sets the type loader.
	/// </summary>
	public Type TypeLoader { get; set; }
	
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
    /// 
    /// </summary>
    /// <param name="options"></param>
    /// <typeparam name="TJob"></typeparam>
    public void AddJob<TJob>(BackgroundJobOptions options)
        where TJob : IJob
    {
	    Jobs[typeof(TJob)] = options;
    }

    /// <summary>
    /// 
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
    /// 
    /// </summary>
    /// <param name="jobType"></param>
    public void AddJob(Type jobType)
    {
        AddJob(jobType, options: null);
    }

    /// <summary>
    /// 
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
}