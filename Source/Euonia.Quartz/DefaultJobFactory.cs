using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Quartz.Spi;

namespace Nerosoft.Euonia.Quartz;

/// <summary>
/// 
/// </summary>
public class DefaultJobFactory : IJobFactory
{
    private readonly IServiceProvider _provider;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="provider"></param>
    public DefaultJobFactory(IServiceProvider provider)
    {
        _provider = provider;
    }

    /// <inheritdoc />
    public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
    {
        var type = bundle.JobDetail.JobType;
        if (type == null)
        {
            throw new NullReferenceException();
        }

        return (IJob)ActivatorUtilities.GetServiceOrCreateInstance(_provider, type);
    }

    /// <inheritdoc />
    public void ReturnJob(IJob job)
    {
        // ReSharper disable once SuspiciousTypeConversion.Global
        if (job is IDisposable disposable)
        {
            disposable.Dispose();
        }
    }
}