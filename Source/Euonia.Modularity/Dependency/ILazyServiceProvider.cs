namespace System;

/// <summary>
/// <see cref="ILazyServiceProvider"/>
/// </summary>
public interface ILazyServiceProvider
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    T GetRequiredService<T>();

    /// <summary>
    /// 
    /// </summary>
    /// <param name="serviceType"></param>
    /// <returns></returns>
    object GetRequiredService(Type serviceType);

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    T GetService<T>();

    /// <summary>
    /// 
    /// </summary>
    /// <param name="serviceType"></param>
    /// <returns></returns>
    object GetService(Type serviceType);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="defaultValue"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    T GetService<T>(T defaultValue);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="serviceType"></param>
    /// <param name="defaultValue"></param>
    /// <returns></returns>
    object GetService(Type serviceType, object defaultValue);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="serviceType"></param>
    /// <param name="factory"></param>
    /// <returns></returns>
    object GetService(Type serviceType, Func<IServiceProvider, object> factory);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="factory"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    T GetService<T>(Func<IServiceProvider, object> factory);
}