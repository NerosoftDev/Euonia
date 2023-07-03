namespace System;

public interface IServiceAccessor : ISingletonDependency
{
    IServiceProvider ServiceProvider { get; set; }
    
    T GetService<T>();

    object GetService(Type type);
}