using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace Nerosoft.Euonia.Hosting;

/// <summary>
/// Server host build utility.
/// </summary>
public class HostUtility
{
    /// <summary>
    /// Run a startup instance.
    /// </summary>
    /// <param name="args"></param>
    /// <param name="optionsAction"></param>
    /// <param name="createHostBuilder"></param>
    /// <typeparam name="TStartup"></typeparam>
    public static void Run<TStartup>(string[] args, Action<HostBuilderOptions> optionsAction = null, Func<string[], HostBuilderOptions, IHostBuilder> createHostBuilder = null)
        where TStartup : class
    {
        var options = new HostBuilderOptions();
        optionsAction?.Invoke(options);
        Run<TStartup>(args, options, createHostBuilder);
    }

    /// <summary>
    /// Run a startup instance.
    /// </summary>
    /// <typeparam name="TStartup"></typeparam>
    /// <param name="args"></param>
    /// <param name="options"></param>
    /// <param name="createHostBuilder"></param>
    public static void Run<TStartup>(string[] args, HostBuilderOptions options, Func<string[], HostBuilderOptions, IHostBuilder> createHostBuilder = null)
        where TStartup : class
    {
        options ??= new HostBuilderOptions();
        if (options.EnableHttp2)
        {
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
        }

        createHostBuilder ??= CreateHostBuilder<TStartup>;

        IHostBuilder builder = createHostBuilder(args, options);

        options.ConfigureHostBuilder?.Invoke(builder);
        builder.Build().Run();
    }

    private static IHostBuilder CreateHostBuilder<TStartup>(string[] args, HostBuilderOptions options)
        where TStartup : class
    {
        var host = Host.CreateDefaultBuilder(args);
        host = host.ConfigureServices((context, _) =>
        {
            Environment.SetEnvironmentVariable(HostBuilderOptions.ApplicationNameVariable, context.HostingEnvironment.ApplicationName);
        });

        if (options.UseAutofac)
        {
            host = host.UseServiceProviderFactory(new AutofacServiceProviderFactory(builder =>
            {
                options?.ConfigureContainerBuilder?.Invoke(builder);
            }));
        }

        if (options.UseSerilog)
        {
            host.ConfigureSerilog();
        }

        host = host.ConfigureWebHostDefaults(builder =>
        {
            builder.UseStartup<TStartup>()
                   .CaptureStartupErrors(options.CaptureStartupErrors);

            options?.ConfigureWebHostBuilder?.Invoke(builder);
        });

        return host;
    }
}