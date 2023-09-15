using Quartz;

namespace Nerosoft.Euonia.Quartz;

/// <summary>
/// The abstract base class for background job schedule attributes.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public abstract class BackgroundJobScheduleAttribute : Attribute
{
	/// <summary>
	/// Initializes a new instance of the <see cref="BackgroundJobScheduleAttribute"/> class.
	/// </summary>
	/// <param name="identity"></param>
	protected BackgroundJobScheduleAttribute(string identity)
	{
		Identity = identity;
	}

	/// <summary>
	/// Gets the trigger identity.
	/// </summary>
	public string Identity { get; }

	/// <summary>
	/// Gets or sets the start delay time in seconds.
	/// 0 means start immediately.
	/// </summary>
	public int Delay { get; set; }

	/// <summary>
	/// Gets or sets the trigger priority.
	/// </summary>
	public int Priority { get; set; }

	/// <summary>
	/// Gets or sets the trigger description.
	/// </summary>
	public string Description { get; set; }

	/// <summary>
	/// Instructs the <see cref="IScheduler" /> that the <see cref="ITrigger"/> will never be evaluated for a misfire situation.
	/// and that the scheduler will simply try to fire it as soon as it can,and then update the Trigger as if it had fired at the proper time.
	/// <br/>
	/// <para><see cref="MisfireInstruction"/>.<see cref="MisfireInstruction.IgnoreMisfirePolicy"/> is valid for all schedule type.</para>
	/// <para>Constants in <see cref="MisfireInstruction"/>.<see cref="MisfireInstruction.CalendarIntervalTrigger"/> area valid for <see cref="CalendarIntervalScheduleAttribute"/></para>
	/// <para>Constants in <see cref="MisfireInstruction"/>.<see cref="MisfireInstruction.CronTrigger"/> area valid for <see cref="CronExpressionScheduleAttribute"/></para>
	/// <para>Constants in <see cref="MisfireInstruction"/>.<see cref="MisfireInstruction.DailyTimeIntervalTrigger"/> area valid for <see cref="DailyTimeIntervalScheduleAttribute"/></para>
	/// <para>Constants in <see cref="MisfireInstruction"/>.<see cref="MisfireInstruction.SimpleTrigger"/> area valid for <see cref="SimpleScheduleAttribute"/></para>
	/// </summary>
	/// <remarks>
	///	Default value: <see cref="MisfireInstruction"/>.<see cref="MisfireInstruction.SmartPolicy"/>.
	/// </remarks>
	public virtual int? MisfirePolicy { get; set; }
	
	/// <summary>
	/// Configures the trigger builder.
	/// </summary>
	/// <returns></returns>
	public abstract IScheduleBuilder Configure();
}