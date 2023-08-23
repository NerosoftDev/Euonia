using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.Elasticsearch;

namespace Nerosoft.Euonia.Hosting;

/// <summary>
/// The extension methods for <see cref="IHostBuilder"/>.
/// </summary>
public static class HostBuilderExtensions
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="host"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static IHostBuilder ConfigureSerilog(this IHostBuilder host)
    {
        if (host == null)
        {
            throw new ArgumentNullException(nameof(host));
        }

        host = host.UseSerilog((context, configuration) =>
        {
            configuration.ReadFrom.Configuration(context.Configuration)
                         .Enrich.FromLogContext()
                         .Enrich.WithProperty("ApplicationName", context.HostingEnvironment.ApplicationName)
                         .Enrich.WithProperty("Environment", context.HostingEnvironment);
            var elasticsearchUrl = context.Configuration["Serilog.Sinks.Elasticsearch:ElasticsearchUrl"];
            if (string.IsNullOrWhiteSpace(elasticsearchUrl))
            {
                configuration.WriteTo.Logger(config => config.Filter.ByIncludingOnly(@event => @event.Level == LogEventLevel.Debug)
                                                             .WriteTo.File("Logs/Debug/logs.log", rollingInterval: RollingInterval.Day))
                             .WriteTo.Logger(config => config.Filter.ByIncludingOnly(@event => @event.Level == LogEventLevel.Warning)
                                                             .WriteTo.File("Logs/Warning/logs.log", rollingInterval: RollingInterval.Day))
                             .WriteTo.Logger(config => config.Filter.ByIncludingOnly(@event => @event.Level == LogEventLevel.Error)
                                                             .WriteTo.File("Logs/Error/logs.log", rollingInterval: RollingInterval.Day))
                             .WriteTo.Logger(config => config.Filter.ByIncludingOnly(@event => @event.Level == LogEventLevel.Information)
                                                             .WriteTo.File("Logs/Info/logs.log", rollingInterval: RollingInterval.Day))
                             .WriteTo.Logger(config => config.Filter.ByIncludingOnly(@event => @event.Level == LogEventLevel.Fatal)
                                                             .WriteTo.File("Logs/Fatal/logs.log", rollingInterval: RollingInterval.Day));
            }
            else
            {
                configuration.WriteTo.Logger(config => config.Filter.ByIncludingOnly(@event => @event.Level == LogEventLevel.Warning))
                             .WriteTo.Logger(config => config.Filter.ByIncludingOnly(@event => @event.Level == LogEventLevel.Error))
                             .WriteTo.Logger(config => config.Filter.ByIncludingOnly(@event => @event.Level == LogEventLevel.Fatal))
                             .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(elasticsearchUrl))
                             {
                                 AutoRegisterTemplate = true,
                                 AutoRegisterTemplateVersion = AutoRegisterTemplateVersion.ESv7,
                                 CustomFormatter = new SerilogElasticsearchFormatter(context.Configuration["Serilog.Sinks.Elasticsearch:HostName"]),
                                 NumberOfReplicas = 0
                             });
            }
        });

        return host;
    }
}
