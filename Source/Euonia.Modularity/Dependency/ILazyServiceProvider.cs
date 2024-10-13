namespace System;

/// <summary>
/// Represents a service provider that can be used to retrieve services with lazy initialization.
/// </summary>
public interface ILazyServiceProvider
{
	/// <summary>
	/// Get service of type <typeparamref name="T"/> from the <see cref="IServiceProvider"/>.
	/// </summary>
	/// <typeparam name="T">The type of service object to get.</typeparam>
	/// <returns></returns>
	T GetRequiredService<T>();

	/// <summary>
	/// Get service of type <paramref name="serviceType"/> from the <see cref="IServiceProvider"/>.
	/// </summary>
	/// <param name="serviceType"></param>
	/// <returns></returns>
	object GetRequiredService(Type serviceType);

	/// <summary>
	/// Get service of type <typeparamref name="T"/> from the <see cref="IServiceProvider"/>.
	/// </summary>
	/// <typeparam name="T">The type of service object to get.</typeparam>
	/// <returns></returns>
	T GetService<T>();

	/// <summary>
	/// Get service of type <paramref name="serviceType"/> from the <see cref="IServiceProvider"/>.
	/// </summary>
	/// <param name="serviceType"></param>
	/// <returns></returns>
	object GetService(Type serviceType);

	/// <summary>
	/// Get service of type <typeparamref name="T"/> from the <see cref="IServiceProvider"/>.
	/// </summary>
	/// <param name="defaultValue"></param>
	/// <typeparam name="T">The type of service object to get.</typeparam>
	/// <returns></returns>
	T GetService<T>(T defaultValue);

	/// <summary>
	/// Get service of type <paramref name="serviceType"/> from the <see cref="IServiceProvider"/>.
	/// </summary>
	/// <param name="serviceType"></param>
	/// <param name="defaultValue"></param>
	/// <returns></returns>
	object GetService(Type serviceType, object defaultValue);

	/// <summary>
	/// Get service of <paramref name="serviceType"/> from the <see cref="IServiceProvider"/>.
	/// </summary>
	/// <param name="serviceType"></param>
	/// <param name="factory"></param>
	/// <returns></returns>
	object GetService(Type serviceType, Func<IServiceProvider, object> factory);

	/// <summary>
	/// Get service of type <typeparamref name="T"/> from the <see cref="IServiceProvider"/>.
	/// </summary>
	/// <param name="factory"></param>
	/// <typeparam name="T">The type of service object to get.</typeparam>
	/// <returns></returns>
	T GetService<T>(Func<IServiceProvider, object> factory);

	/// <summary>
	/// Get service of <paramref name="serviceType"/> from the <see cref="IServiceProvider"/>.
	/// </summary>
	/// <param name="serviceType">An object that specifies the type of service object to get.</param>
	/// <param name="serviceKey">An object that specifies the key of service object to get.</param>
	/// <returns></returns>
	object GetKeyedService(Type serviceType, object serviceKey);

	/// <summary>
	/// Get service of type <typeparamref name="T"/> from the <see cref="IServiceProvider"/>.
	/// </summary>
	/// <typeparam name="T">The type of service object to get.</typeparam>
	/// <param name="serviceKey">An object that specifies the key of service object to get.</param>
	/// <returns></returns>
	T GetRequiredKeyedService<T>(object serviceKey);

	/// <summary>
	/// Get service of type <typeparamref name="T"/> from the <see cref="IServiceProvider"/>.
	/// </summary>
	/// <typeparam name="T">The type of service object to get.</typeparam>
	/// <param name="serviceKey">An object that specifies the key of service object to get.</param>
	/// <returns></returns>
	T GetKeyedService<T>(object serviceKey);

	/// <summary>
	/// Get service of type <paramref name="serviceType"/> from the <see cref="IServiceProvider"/>.
	/// </summary>
	/// <param name="serviceType">An object that specifies the type of service object to get.</param>
	/// <param name="serviceKey">An object that specifies the key of service object to get.</param>
	/// <returns></returns>
	object GetRequiredKeyedService(Type serviceType, object serviceKey);

	/// <summary>
	/// Get service of type <typeparamref name="T"/> from the <see cref="IServiceProvider"/>.
	/// </summary>
	/// <typeparam name="T">The type of service object to get.</typeparam>
	/// <param name="serviceKey">An object that specifies the key of service object to get.</param>
	/// <param name="defaultValue"></param>
	/// <returns></returns>
	T GetKeyedService<T>(object serviceKey, T defaultValue);

	/// <summary>
	/// Get service of type <typeparamref name="T"/> from the <see cref="IServiceProvider"/>.
	/// </summary>
	/// <typeparam name="T">The type of service object to get.</typeparam>
	/// <param name="serviceKey">An object that specifies the key of service object to get.</param>
	/// <param name="factory"></param>
	/// <returns></returns>
	T GetKeyedService<T>(object serviceKey, Func<IServiceProvider, T> factory);

	/// <summary>
	/// Get service of type <paramref name="serviceType"/> from the <see cref="IServiceProvider"/>.
	/// </summary>
	/// <param name="serviceType">An object that specifies the type of service object to get.</param>
	/// <param name="serviceKey">An object that specifies the key of service object to get.</param>
	/// <param name="factory"></param>
	/// <returns></returns>
	object GetKeyedService(Type serviceType, object serviceKey, Func<IServiceProvider, object> factory);

	/// <summary>
	/// Get service of type <paramref name="serviceType"/> from the <see cref="IServiceProvider"/>.
	/// </summary>
	/// <param name="serviceType">An object that specifies the type of service object to get.</param>
	/// <param name="serviceKey">An object that specifies the key of service object to get.</param>
	/// <param name="defaultValue"></param>
	/// <returns></returns>
	object GetKeyedService(Type serviceType, object serviceKey, object defaultValue);
}