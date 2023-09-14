using System.Globalization;
using Quartz;

namespace Nerosoft.Euonia.Quartz;

/// <summary>
/// Represents a cron expression trigger attribute.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class CronExpressionScheduleAttribute : BackgroundJobScheduleAttribute
{
	/// <summary>
	/// Initializes a new instance of the <see cref="CronExpressionScheduleAttribute"/> class.
	/// </summary>
	/// <param name="identity">The trigger identity.</param>
	/// <param name="cronExpression">The cron expression for schedule.</param>
	public CronExpressionScheduleAttribute(string identity, string cronExpression)
		: base(identity)
	{
		if (string.IsNullOrWhiteSpace(cronExpression))
		{
			throw new ArgumentNullException(nameof(cronExpression));
		}

		CronExpression = cronExpression;
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="CronExpressionScheduleAttribute"/> class.
	/// </summary>
	/// <param name="identity"></param>
	/// <param name="hour"></param>
	/// <param name="minute"></param>
	/// <param name="daysOfWeek"></param>
	public CronExpressionScheduleAttribute(string identity, int hour, int minute, params DayOfWeek[] daysOfWeek)
		: base(identity)
	{
		DateBuilder.ValidateHour(hour);
		DateBuilder.ValidateMinute(minute);

		if (daysOfWeek?.Length > 0)
		{
			var abbreviations = daysOfWeek.Select(GetAbbreviation).JoinAsString(",");
			CronExpression = $"0 {minute} {hour} ? * {abbreviations}";
		}
		else
		{
			CronExpression = $"0 {minute} {hour} ? * *";
		}
	}

	/// <summary>
	/// Initializes a new instance of the &lt;see cref="CronExpressionScheduleAttribute"/&gt; class.
	/// </summary>
	/// <param name="identity"></param>
	/// <param name="hour"></param>
	/// <param name="minute"></param>
	/// <param name="dayOfMonth"></param>
	public CronExpressionScheduleAttribute(string identity, int hour, int minute, int dayOfMonth)
		: base(identity)
	{
		DateBuilder.ValidateDayOfMonth(dayOfMonth);
		DateBuilder.ValidateHour(hour);
		DateBuilder.ValidateMinute(minute);

		CronExpression = $"0 {minute} {hour} {dayOfMonth} * ?";
	}

	/// <summary>
	/// Gets the crontab expression.
	/// </summary>
	public string CronExpression { get; }

	/// <summary>
	/// Gets or sets the job time zone name.
	/// Please check https://docs.microsoft.com/en-us/dotnet/standard/base-types/time-zone-names-and-ids
	/// Or you can get the time zone names using method <see cref="TimeZoneInfo.GetSystemTimeZones"/>
	/// </summary>
	public string TimeZoneName { get; set; } = "UTC";

	/// <summary>
	/// Instructs the <see cref="IScheduler" /> that the <see cref="ITrigger"/> will never be evaluated for a misfire situation.
	/// and that the scheduler will simply try to fire it as soon as it can,and then update the Trigger as if it had fired at the proper time.
	/// <para><see cref="MisfireInstruction.IgnoreMisfirePolicy"/> - WithMisfireHandlingInstructionIgnoreMisfires;</para>
	/// <para><see cref="MisfireInstruction.CronTrigger.DoNothing"/> - WithMisfireHandlingInstructionDoNothing;</para>
	/// <para><see cref="MisfireInstruction.CronTrigger.FireOnceNow"/> - WithMisfireHandlingInstructionFireAndProceed;</para>
	/// <para>Others - Use default value <see cref="MisfireInstruction.SmartPolicy"/>.</para>
	/// </summary>
	public int? MisfirePolicy { get; set; }

	/// <inheritdoc/>
	public override IScheduleBuilder Configure()
	{
		var builder = CronScheduleBuilder.CronSchedule(CronExpression);

		var timeZoneInfo = TimeZoneName.ToUpper(CultureInfo.CurrentCulture) switch
		{
			"UTC" => TimeZoneInfo.Utc,
			"LOCAL" => TimeZoneInfo.Local,
			_ => TimeZoneInfo.FindSystemTimeZoneById(TimeZoneName)
		};
		builder.InTimeZone(timeZoneInfo);

		switch (MisfirePolicy)
		{
			case MisfireInstruction.IgnoreMisfirePolicy:
				builder.WithMisfireHandlingInstructionIgnoreMisfires();
				break;
			case MisfireInstruction.CronTrigger.DoNothing:
				builder.WithMisfireHandlingInstructionDoNothing();
				break;
			case MisfireInstruction.CronTrigger.FireOnceNow:
				builder.WithMisfireHandlingInstructionFireAndProceed();
				break;
		}

		return builder;
	}

	/// <summary>
	/// Gets the abbreviation of <see cref="DayOfWeek"/>.
	/// </summary>
	/// <param name="dayOfWeek"></param>
	/// <returns>The abbreviated day name (e.g. SUN,MON,TUE,WED,THU,FRI,SAT) of specified day of week.</returns>
	private static string GetAbbreviation(DayOfWeek dayOfWeek)
	{
		return dayOfWeek switch
		{
			DayOfWeek.Sunday => "SUN",
			DayOfWeek.Monday => "MON",
			DayOfWeek.Tuesday => "TUE",
			DayOfWeek.Wednesday => "WED",
			DayOfWeek.Thursday => "THU",
			DayOfWeek.Friday => "FRI",
			DayOfWeek.Saturday => "SAT",
			_ => ((int)dayOfWeek).ToString()
		};
	}
}