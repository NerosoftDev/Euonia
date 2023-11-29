using System.ComponentModel;
using System.Globalization;
using System.Reflection;

public static partial class Extensions
{
	/// <summary>
	/// Used to simplify and beautify casting an object to a type.
	/// </summary>
	/// <typeparam name="T">Type to be casted</typeparam>
	/// <param name="obj">Object to cast</param>
	/// <returns>Casted object</returns>
	public static T As<T>(this object obj)
		where T : class
	{
		return (T)obj;
	}

	/// <summary>
	/// Converts given object to a value type using <see cref="Convert.ChangeType(object,System.Type)"/> method.
	/// </summary>
	/// <param name="obj">Object to be converted</param>
	/// <typeparam name="T">Type of the target object</typeparam>
	/// <returns>Converted object</returns>
	public static T To<T>(this object obj)
		where T : struct
	{
		if (obj == null)
		{
			throw new NullReferenceException();
		}

		if (typeof(T) == obj.GetType())
		{
			return (T)obj;
		}

		if (typeof(T) == typeof(Guid))
		{
			// ReSharper disable once PossibleNullReferenceException
			return (T)TypeDescriptor.GetConverter(typeof(T)).ConvertFromInvariantString(obj.ToString());
		}

		return (T)System.Convert.ChangeType(obj, typeof(T), CultureInfo.InvariantCulture);
	}

	/// <summary>
	/// Check if an item is in a list.
	/// </summary>
	/// <param name="item">Item to check</param>
	/// <param name="list">List of items</param>
	/// <typeparam name="T">Type of the items</typeparam>
	public static bool IsIn<T>(this T item, params T[] list)
	{
		return list.Contains(item);
	}

	/// <summary>
	/// Check if an item is in the given enumerable.
	/// </summary>
	/// <param name="item">Item to check</param>
	/// <param name="items">Items</param>
	/// <typeparam name="T">Type of the items</typeparam>
	public static bool IsIn<T>(this T item, IEnumerable<T> items)
	{
		return items.Contains(item);
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="item"></param>
	/// <param name="items"></param>
	/// <param name="comparer"></param>
	/// <typeparam name="T"></typeparam>
	/// <returns></returns>
	public static bool IsIn<T>(this T item, IEnumerable<T> items, IEqualityComparer<T> comparer)
	{
		return items.Contains(item, comparer);
	}

	/// <summary>
	/// Can be used to conditionally perform a function
	/// on an object and return the modified or the original object.
	/// It is useful for chained calls.
	/// </summary>
	/// <param name="obj">An object</param>
	/// <param name="condition">A condition</param>
	/// <param name="func">A function that is executed only if the condition is <code>true</code></param>
	/// <typeparam name="T">Type of the object</typeparam>
	/// <returns>
	/// Returns the modified object (by the <paramref name="func"/> if the <paramref name="condition"/> is <code>true</code>)
	/// or the original object if the <paramref name="condition"/> is <code>false</code>
	/// </returns>
	public static T If<T>(this T obj, bool condition, Func<T, T> func)
	{
		return condition ? func(obj) : obj;
	}

	/// <summary>
	/// Can be used to conditionally perform an action
	/// on an object and return the original object.
	/// It is useful for chained calls on the object.
	/// </summary>
	/// <param name="obj">An object</param>
	/// <param name="condition">A condition</param>
	/// <param name="action">An action that is executed only if the condition is <code>true</code></param>
	/// <typeparam name="T">Type of the object</typeparam>
	/// <returns>
	/// Returns the original object.
	/// </returns>
	public static T If<T>(this T obj, bool condition, Action<T> action)
	{
		if (condition)
		{
			action(obj);
		}

		return obj;
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="type"></param>
	/// <param name="inherit"></param>
	/// <typeparam name="TAttribute"></typeparam>
	/// <returns></returns>
	public static bool HasAttribute<TAttribute>(this Type type, bool inherit = true)
		where TAttribute : Attribute
	{
		var attribute = type.GetCustomAttributes<TAttribute>(inherit);
		return attribute.Any();
	}

	/// <summary>
	/// 
	/// </summary>
	/// <typeparam name="TAttribute"></typeparam>
	/// <param name="method"></param>
	/// <param name="inherit"></param>
	/// <returns></returns>
	public static bool HasAttribute<TAttribute>(this MethodInfo method, bool inherit = true)
		where TAttribute : Attribute
	{
		var attribute = method.GetCustomAttributes<TAttribute>(inherit);
		return attribute.Any();
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="source"></param>
	/// <param name="attribute"></param>
	/// <param name="inherit"></param>
	/// <typeparam name="TAttribute"></typeparam>
	/// <returns></returns>
	public static bool HasAttribute<TAttribute>(this object source, out TAttribute attribute, bool inherit = true)
		where TAttribute : Attribute
	{
		var type = source.GetType();
		attribute = type.GetCustomAttribute<TAttribute>(inherit);
		return attribute != null;
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="source"></param>
	/// <param name="attributes"></param>
	/// <param name="inherit"></param>
	/// <typeparam name="TAttribute"></typeparam>
	/// <returns></returns>
	public static bool HasAttribute<TAttribute>(this object source, out IEnumerable<TAttribute> attributes, bool inherit = true)
		where TAttribute : Attribute
	{
		var type = source.GetType();
		attributes = type.GetCustomAttributes<TAttribute>(inherit);
		return attributes.Any();
	}
}
