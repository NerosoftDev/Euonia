using System.Security.Claims;
using System.Security.Principal;
using Microsoft.Extensions.DependencyInjection;
using Nerosoft.Euonia.Claims;

namespace Nerosoft.Euonia.Business;

/// <summary>
/// 
/// </summary>
public class BusinessContext
{
	/// <summary>
	/// Initializes a new instance of the <see cref="BusinessContext"/> class.
	/// </summary>
	/// <param name="contextAccessor"></param>
	public BusinessContext(BusinessContextAccessor contextAccessor)
	{
		ContextAccessor = contextAccessor;
		User = contextAccessor.ServiceProvider.GetService<UserPrincipal>();
	}

	/// <summary>
	/// Get the BusinessContextAccessor object.
	/// </summary>
	internal BusinessContextAccessor ContextAccessor { get; }

	/// <summary>
	/// Get or set the current ClaimsPrincipal object representing the user's identity.
	/// </summary>
	public ClaimsPrincipal Principal => User?.Claims;

	/// <summary>
	/// Get or set the current <see cref="IPrincipal" /> object representing the user's identity.
	/// </summary>
	public UserPrincipal User { get; set; }

	/// <summary>
	/// Get the current service provider.
	/// </summary>
	public IServiceProvider CurrentServiceProvider => ContextAccessor.ServiceProvider;

	/// <summary>
	/// Get the service object of the specified type.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <returns></returns>
	/// <exception cref="NullReferenceException"></exception>
	public T GetRequiredService<T>()
	{
		if (CurrentServiceProvider == null)
		{
			throw new NullReferenceException(nameof(CurrentServiceProvider));
		}

		var result = CurrentServiceProvider.GetRequiredService<T>();
		return result;
	}

	/// <summary>
	/// Get the service object of the specified type.
	/// </summary>
	/// <param name="serviceType"></param>
	/// <returns></returns>
	/// <exception cref="NullReferenceException"></exception>
	public object GetRequiredService(Type serviceType)
	{
		if (CurrentServiceProvider == null)
		{
			throw new NullReferenceException(nameof(CurrentServiceProvider));
		}

		return CurrentServiceProvider.GetRequiredService(serviceType);
	}

	/// <summary>
	/// Create an instance of the specified type using the constructor that best matches the specified parameters.
	/// </summary>
	/// <param name="parameters"></param>
	/// <typeparam name="T"></typeparam>
	/// <returns></returns>
	public T CreateInstance<T>(params object[] parameters)
	{
		return (T)CreateInstance(typeof(T), parameters);
	}

	/// <summary>
	/// Create an instance of the specified type using the constructor that best matches the specified parameters.
	/// </summary>
	/// <param name="objectType"></param>
	/// <param name="parameters"></param>
	/// <returns></returns>
	public object CreateInstance(Type objectType, params object[] parameters)
	{
		object result;
		if (CurrentServiceProvider != null)
		{
			result = ActivatorUtilities.CreateInstance(CurrentServiceProvider, objectType, parameters);
		}
		else
		{
			result = Activator.CreateInstance(objectType, parameters);
		}

		if (result is IUseBusinessContext tmp)
		{
			tmp.BusinessContext = this;
		}

		return result;
	}

	/// <summary>
	/// Create an instance of the specified type using the constructor that best matches the specified parameters.
	/// </summary>
	/// <param name="type"></param>
	/// <param name="paramTypes"></param>
	/// <returns></returns>
	internal object CreateGenericInstance(Type type, params Type[] paramTypes)
	{
		var genericType = type.GetGenericTypeDefinition();
		var gt = genericType.MakeGenericType(paramTypes);
		return CreateInstance(gt);
	}
}