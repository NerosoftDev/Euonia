using Quartz;

namespace Nerosoft.Euonia.Quartz;

/// <summary>
/// The background job options.
/// </summary>
public class BackgroundJobOptions
{
    /// <summary>
    /// Gets the job name.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the quartz job group name.
    /// </summary>
    public string Group { get; set; }

    /// <summary>
    /// Gets the crontab expression.
    /// </summary>
    public string CronExpression { get; set; }

    /// <summary>
    /// Gets or sets the job time zone name.
    /// Please check https://docs.microsoft.com/en-us/dotnet/standard/base-types/time-zone-names-and-ids
    /// Or you can get the time zone names using method <see cref="TimeZoneInfo.GetSystemTimeZones"/>
    /// </summary>
    public string TimeZoneName { get; set; } = "UTC";

    /// <summary>
    /// Gets or sets the <see cref="TriggerBuilder"/> action.
    /// </summary>
    public Func<TriggerBuilder, TriggerBuilder> Action { get; set; }
}