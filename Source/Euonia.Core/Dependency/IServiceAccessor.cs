namespace System;

/// <summary>
/// Accessing services from the IoC container.
/// </summary>
public interface IServiceAccessor : ISingletonDependency
{
    /// <summary>
    /// Gets or sets the service provider.
    /// </summary>
    IServiceProvider ServiceProvider { get; set; }

    /// <summary>
    /// Resolves an instance of the requested type from the <see cref="ServiceProvider"/>.
    /// </summary>
    /// <typeparam name="T">The service type.</typeparam>
    /// <returns></returns>
    T GetService<T>();

    /// <summary>
    /// Resolves an instance of the requested type from the <see cref="ServiceProvider"/>.
    /// </summary>
    /// <param name="type">The service type.</param>
    /// <returns></returns>
    object GetService(Type type);
}