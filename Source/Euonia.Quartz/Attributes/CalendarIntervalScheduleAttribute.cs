using Quartz;

namespace Nerosoft.Euonia.Quartz;

/// <summary>
/// Represents a calendar-based trigger, which is based on repeating calendar time intervals.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class CalendarIntervalScheduleAttribute : BackgroundJobScheduleAttribute
{
	/// <summary>
	/// Initializes a new instance of the <see cref="CalendarIntervalScheduleAttribute"/> class.
	/// </summary>
	/// <param name="identity"></param>
	/// <param name="interval"></param>
	/// <param name="unit"></param>
	public CalendarIntervalScheduleAttribute(string identity, int interval, IntervalUnit unit)
		: base(identity)
	{
		Interval = interval;
		Unit = unit;
	}

	/// <summary>
	/// Gets the interval at which the trigger should repeat.
	/// </summary>
	public int Interval { get; }

	/// <summary>
	/// Gets the time unit of the interval.
	/// </summary>
	public IntervalUnit Unit { get; }

	/// <summary>
	/// Instructs the <see cref="IScheduler" /> that the <see cref="ITrigger"/> will never be evaluated for a misfire situation.
	/// and that the scheduler will simply try to fire it as soon as it can,and then update the Trigger as if it had fired at the proper time.
	/// <para><see cref="MisfireInstruction.IgnoreMisfirePolicy"/> - WithMisfireHandlingInstructionIgnoreMisfires;</para>
	/// <para><see cref="MisfireInstruction.CalendarIntervalTrigger.DoNothing"/> - WithMisfireHandlingInstructionDoNothing;</para>
	/// <para><see cref="MisfireInstruction.CalendarIntervalTrigger.FireOnceNow"/> - WithMisfireHandlingInstructionFireAndProceed;</para>
	/// <para>Others - Use default value <see cref="MisfireInstruction.SmartPolicy"/>.</para>
	/// </summary>
	public int? MisfirePolicy { get; set; }

	/// <summary>
	/// 
	/// </summary>
	public bool? SkipDayIfHourDoesNotExist { get; set; }

	/// <summary>
	/// 
	/// </summary>
	public bool? PreserveHourOfDayAcrossDaylightSavings { get; set; }

	/// <inheritdoc />
	public override IScheduleBuilder Configure()
	{
		var builder = CalendarIntervalScheduleBuilder.Create();
		builder.WithInterval(Interval, Unit);
		switch (MisfirePolicy)
		{
			case MisfireInstruction.IgnoreMisfirePolicy:
				builder.WithMisfireHandlingInstructionIgnoreMisfires();
				break;
			case MisfireInstruction.CalendarIntervalTrigger.DoNothing:
				builder.WithMisfireHandlingInstructionDoNothing();
				break;
			case MisfireInstruction.CalendarIntervalTrigger.FireOnceNow:
				builder.WithMisfireHandlingInstructionFireAndProceed();
				break;
		}
		if (SkipDayIfHourDoesNotExist.HasValue)
		{
			builder.SkipDayIfHourDoesNotExist(SkipDayIfHourDoesNotExist.Value);
		}
		if (PreserveHourOfDayAcrossDaylightSavings.HasValue)
		{
			builder.PreserveHourOfDayAcrossDaylightSavings(PreserveHourOfDayAcrossDaylightSavings.Value);
		}
		return builder;
	}
}
