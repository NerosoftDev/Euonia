namespace System;

public class ServiceAccessor : IServiceAccessor
{
    private static readonly AsyncLocal<IServiceProvider> _provider = new();

    public IServiceProvider ServiceProvider
    {
        get => _provider.Value;
        set => _provider.Value = value;
    }

    public T GetService<T>()
    {
        return (T)GetService(typeof(T));
    }

    public object GetService(Type type)
    {
        return _provider.Value?.GetService(type);
    }
}