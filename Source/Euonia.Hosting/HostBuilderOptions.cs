using System.Reflection;
using Autofac;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace Nerosoft.Euonia.Hosting;

/// <summary>
/// The options to build server host.
/// </summary>
public class HostBuilderOptions
{
    /// <summary>
    /// Gets the environment variable to set the application name.
    /// </summary>
    public const string ApplicationNameVariable = "SERVICE_NAME";

    /// <summary>
    /// Gets or sets a value to indicate whether the HTTP/2 protocal is enabled in the application
    /// </summary>
    public bool EnableHttp2 { get; set; } = true;

    /// <summary>
    /// Gets or sets a value to indicate whether the exception should be caught while application startup.
    /// </summary>
    public bool CaptureStartupErrors { get; set; } = true;

    /// <summary>
    /// Gets or sets a value to indicate whether the application will use autofac as service container provider.
    /// </summary>
    public bool UseAutofac { get; set; } = true;

    /// <summary>
    /// Gets or sets a value to indicate whether the application will use Serilog as logging service provider.
    /// </summary>
    public bool UseSerilog { get; set; } = true;

    /// <summary>
    /// Gets or sets the application name.
    /// </summary>
    public object ApplicationName { get; set; } = Assembly.GetEntryAssembly()?.GetName();

    /// <summary>
    /// Gets or sets handle action for <see cref="IWebHostBuilder"/>.
    /// </summary>
    public Action<IWebHostBuilder> ConfigureWebHostBuilder { get; set; }

    /// <summary>
    /// Gets or sets handle action for <see cref="ContainerBuilder"/>
    /// </summary>
    public Action<ContainerBuilder> ConfigureContainerBuilder { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public Action<IHostBuilder> ConfigureHostBuilder { get; set; }
}
