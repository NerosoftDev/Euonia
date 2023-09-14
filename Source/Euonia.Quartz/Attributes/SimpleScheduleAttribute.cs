using Quartz;

namespace Nerosoft.Euonia.Quartz;

/// <summary>
/// 
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class SimpleScheduleAttribute : BackgroundJobScheduleAttribute
{
	/// <summary>
	/// Initializes a new instance of the <see cref="SimpleScheduleAttribute"/> class.
	/// </summary>
	/// <param name="identity"></param>
	/// <param name="milliseconds"></param>
	public SimpleScheduleAttribute(string identity, int milliseconds)
		: base(identity)
	{
		Interval = TimeSpan.FromMilliseconds(milliseconds);
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="SimpleScheduleAttribute"/> class.
	/// </summary>
	/// <param name="identity"></param>
	/// <param name="hour"></param>
	/// <param name="minute"></param>
	/// <param name="second"></param>
	public SimpleScheduleAttribute(string identity, int hour, int minute, int second)
		: base(identity)
	{
		Interval = new TimeSpan(hour, minute, second);
	}

	/// <summary>
	/// Gets the trigger interval.
	/// </summary>
	public TimeSpan Interval { get; }

	/// <summary>
	/// Gets or sets the number of time that the trigger will repeat.
	/// Total number of firings will be this number + 1.
	/// Leave it null if you want to repeat forever.
	/// </summary>
	public int? RepeatCount { get; set; }

	/// <summary>
	/// Instructs the <see cref="IScheduler" /> that the <see cref="ITrigger"/> will never be evaluated for a misfire situation.
	/// and that the scheduler will simply try to fire it as soon as it can,and then update the Trigger as if it had fired at the proper time.
	/// </summary>
	public int? MisfirePolicy { get; set; }

	/// <inheritdoc/>
	public override IScheduleBuilder Configure()
	{
		var builder = SimpleScheduleBuilder.Create();

		builder.WithInterval(Interval);
		switch (RepeatCount)
		{
			case null:
				builder.RepeatForever();
				break;
			case >= 0:
				builder.WithRepeatCount(RepeatCount.Value);
				break;
			case < 0:
				throw new InvalidOperationException("RepeatCount must be greater than or equal to 0. Leave it null if you want to repeat forever.");

		}
		if (RepeatCount >= 0)
		{
			builder.WithRepeatCount(RepeatCount.Value);
		}
		else
		{
			builder.RepeatForever();
		}
		switch (MisfirePolicy)
		{
			case MisfireInstruction.SimpleTrigger.FireNow:
				builder.WithMisfireHandlingInstructionFireNow();
				break;
			case MisfireInstruction.SimpleTrigger.RescheduleNowWithExistingRepeatCount:
				builder.WithMisfireHandlingInstructionNowWithExistingCount();
				break;
			case MisfireInstruction.SimpleTrigger.RescheduleNowWithRemainingRepeatCount:
				builder.WithMisfireHandlingInstructionNowWithRemainingCount();
				break;
			case MisfireInstruction.SimpleTrigger.RescheduleNextWithRemainingCount:
				builder.WithMisfireHandlingInstructionNextWithRemainingCount();
				break;
			case MisfireInstruction.SimpleTrigger.RescheduleNextWithExistingCount:
				builder.WithMisfireHandlingInstructionNextWithExistingCount();
				break;
		}

		return builder;
	}
}
