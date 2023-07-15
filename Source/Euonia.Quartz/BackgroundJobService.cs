using Microsoft.Extensions.Hosting;
using System.Globalization;
using System.Reflection;
using Microsoft.Extensions.Options;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;

namespace Nerosoft.Euonia.Quartz;

/// <summary>
/// 
/// </summary>
public class BackgroundJobService : BackgroundService
{
    private readonly IScheduler _scheduler;
    private readonly BackgroundBuildOptions _options;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="jobFactory"></param>
    /// <param name="options"></param>
    public BackgroundJobService(IJobFactory jobFactory, IOptions<BackgroundBuildOptions> options)
    {
        _options = options.Value;
        var factory = new StdSchedulerFactory();
        _scheduler = factory.GetScheduler().Result; //AsyncHelper.RunSync(async () => await factory.GetScheduler());
        _scheduler.JobFactory = jobFactory;
    }

    /// <inheritdoc />
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await _scheduler.Start(stoppingToken);

        foreach (var (type, itemOptions) in _options.Jobs)
        {
            var attribute = type.GetCustomAttribute<BackgroundJobAttribute>();

            var cron = itemOptions?.CronExpression ?? attribute?.CronExpression;
            var name = itemOptions?.Name ?? attribute?.Name;
            var group = itemOptions?.Group ?? attribute?.Group;
            var timeZoneName = (itemOptions?.TimeZoneName ?? attribute?.TimeZoneName) ?? "UTC";

            if (string.IsNullOrWhiteSpace(cron) || string.IsNullOrWhiteSpace(name))
            {
                continue;
            }

            var timeZoneInfo = timeZoneName.ToUpper(CultureInfo.CurrentCulture) switch
            {
                "UTC" => TimeZoneInfo.Utc,
                "LOCAL" => TimeZoneInfo.Local,
                _ => TimeZoneInfo.FindSystemTimeZoneById(timeZoneName)
            };

            var job = JobBuilder.Create(type).Build();
            var builder = TriggerBuilder.Create()
                                        .WithIdentity(name, group ?? "default")
                                        .StartNow()
                                        .WithCronSchedule(cron, x => x.InTimeZone(timeZoneInfo));

            if (itemOptions?.Action != null)
            {
                builder = itemOptions.Action(builder);
            }

            //.WithCronSchedule(attribute.CronConfigure, x => x.InTimeZone(TimeZoneInfo.FromSerializedString("China Standard Time")))
            //.UsingJobData("Code", type.Code)
            //.UsingJobData("Key", _configuration["Juhe:Key"])
            //.UsingJobData("Url", _configuration["Juhe:Url"])

            var trigger = builder.Build();

            await _scheduler.ScheduleJob(job, trigger, stoppingToken);
        }
    }

    /// <inheritdoc />
    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        await _scheduler.Shutdown(cancellationToken);
        await _scheduler.Clear(cancellationToken);
        await base.StopAsync(cancellationToken);
    }
}