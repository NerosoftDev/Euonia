﻿using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Nerosoft.Euonia.Application;

/// <summary>
/// The service context base.
/// </summary>
public abstract class ServiceContextBase : IServiceContext
{
    /// <inheritdoc />
    public Assembly Assembly => Assembly.GetExecutingAssembly();

    /// <inheritdoc />
    public virtual bool AutoRegisterApplicationService => true;

    /// <inheritdoc />
    public virtual void ConfigureServices(IServiceCollection services)
    {
    }
}