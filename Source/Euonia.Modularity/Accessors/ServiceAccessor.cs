namespace System;

/// <summary>
/// Implements the <see cref="IServiceAccessor"/> interface to provides a way to access services.
/// </summary>
public class ServiceAccessor : IServiceAccessor
{
    private static readonly AsyncLocal<IServiceProvider> _provider = new();

    /// <inheritdoc/>
    public IServiceProvider ServiceProvider
    {
        get => _provider.Value;
        set => _provider.Value = value;
    }

    /// <inheritdoc/>
    public T GetService<T>()
    {
        return (T)GetService(typeof(T));
    }

    /// <inheritdoc/>
    public object GetService(Type type)
    {
        return _provider.Value?.GetService(type);
    }
}