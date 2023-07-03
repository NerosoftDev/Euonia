using Microsoft.Extensions.DependencyInjection;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedType.Global

namespace Nerosoft.Euonia.Modularity;

/// <summary>
/// 
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class ExportServiceAttribute : Attribute
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="lifetime"></param>
    public ExportServiceAttribute(ServiceLifetime lifetime)
    {
        Lifetime = lifetime;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="lifetime"></param>
    /// <param name="serviceTypes"></param>
    /// <exception cref="RegistrationException"></exception>
    public ExportServiceAttribute(ServiceLifetime lifetime, params Type[] serviceTypes)
        : this(lifetime)
    {
        Lifetime = lifetime;

        if (serviceTypes is not { Length: > 0 })
        {
            return;
        }

        foreach (var serviceType in serviceTypes)
        {
            if (serviceType == null)
            {
                continue;
            }

            if (!serviceType.IsClass && !serviceType.IsInterface)
            {
                throw new InvalidOperationException("Only interface or class can be registered as service.");
            }
        }

        ServiceTypes = new List<Type>(serviceTypes);
    }

    /// <summary>
    /// Gets the service lifetime.
    /// </summary>
    public ServiceLifetime Lifetime { get; }

    /// <summary>
    /// Gets the implemented service types.
    /// </summary>
    public List<Type> ServiceTypes { get; }

    /// <summary>
    /// 
    /// </summary>
    public virtual bool TryRegister { get; set; }

    /// <summary>
    /// 
    /// </summary>
    public virtual bool ReplaceServices { get; set; }
}

/// <summary>
/// 
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class ExportServiceAsTransientAttribute : ExportServiceAttribute
{
    /// <summary>
    /// Initialize a new instance of <see cref="ExportServiceAsTransientAttribute"/>.
    /// </summary>
    public ExportServiceAsTransientAttribute()
        : base(ServiceLifetime.Transient)
    {
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="serviceType"></param>
    public ExportServiceAsTransientAttribute(Type serviceType)
        : base(ServiceLifetime.Transient, serviceType)
    {
    }
}

/// <summary>
/// 
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class ExportServiceAsSingletonAttribute : ExportServiceAttribute
{
    /// <summary>
    /// 
    /// </summary>
    public ExportServiceAsSingletonAttribute()
        : base(ServiceLifetime.Singleton)
    {
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="serviceType"></param>
    public ExportServiceAsSingletonAttribute(Type serviceType)
        : base(ServiceLifetime.Singleton, serviceType)
    {
    }
}

/// <summary>
/// 
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class ExportServiceAsScopedAttribute : ExportServiceAttribute
{
    /// <summary>
    /// 
    /// </summary>
    public ExportServiceAsScopedAttribute()
        : base(ServiceLifetime.Scoped)
    {
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="serviceType"></param>
    public ExportServiceAsScopedAttribute(Type serviceType)
        : base(ServiceLifetime.Scoped, serviceType)
    {
    }
}