namespace Nerosoft.Euonia.Quartz;

/// <summary>
/// The background job attribute.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class BackgroundJobAttribute : Attribute
{
    /// <summary>
    /// Initialize a new instance of <see cref="BackgroundJobAttribute"/>.
    /// </summary>
    /// <param name="name">The job name.</param>
    /// <param name="cron">The job schedule crontab expression.</param>
    /// <exception cref="ArgumentNullException"></exception>
    public BackgroundJobAttribute(string name, string cron)
    {
        Name = name;
        if (string.IsNullOrWhiteSpace(cron))
        {
            throw new ArgumentNullException(nameof(cron));
        }

        CronExpression = cron;
    }

    /// <summary>
    /// Gets the job name.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets or sets the quartz job group name.
    /// </summary>
    public string Group { get; set; }

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
}