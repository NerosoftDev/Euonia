namespace System;

/// <summary>
/// The extension methods for <see cref="IServiceProvider"/> interface.
/// </summary>
public static class ServiceProviderExtensions
{
    /// <summary>
    /// Gets the service object with specified name.
    /// </summary>
    /// <param name="provider">The service provider instance.</param>
    /// <param name="name">The registered service name.</param>
    /// <typeparam name="TService">The service type.</typeparam>
    /// <returns></returns>
    public static TService GetService<TService>(this IServiceProvider provider, string name)
        where TService : class
    {
        var @delegate = (NamedService<TService>)provider.GetService(typeof(NamedService<TService>));
        return @delegate?.Invoke(name);
    }

    /// <summary>
    /// Gets the service object with specified name.
    /// </summary>
    /// <param name="provider">The service provider instance.</param>
    /// <param name="name">The registered service name.</param>
    /// <typeparam name="TService">The service type.</typeparam>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException">Throws if service with specified name was not resolved.</exception>
    public static TService GetRequiredService<TService>(this IServiceProvider provider, string name)
        where TService : class
    {
        var @delegate = (NamedService<TService>)provider.GetService(typeof(NamedService<TService>));
        return @delegate?.Invoke(name) ?? throw new InvalidOperationException($"The service {typeof(TService).FullName} with name {name} was not found.");
    }
}
