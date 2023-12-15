﻿using System.Collections.Concurrent;
using System.Reflection;

namespace Nerosoft.Euonia.Business;

/// <summary>
/// The object reflector.
/// </summary>
public class ObjectReflector
{
	private const BindingFlags BINDING_FLAGS = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly;

	private static readonly string[] _collectionTypesName =
	{
		typeof(IList<>).FullName,
		typeof(ICollection<>).FullName,
		typeof(IEnumerable<>).FullName,
	};

	private static readonly ConcurrentDictionary<Type, List<Tuple<PropertyInfo, Type, bool>>> _propertyCache = new();
	private static readonly ConcurrentDictionary<string, MethodInfo> _factoryMethods = new();

	internal static List<Tuple<PropertyInfo, Type, bool>> GetAutoInjectProperties(Type objectType)
	{
		return _propertyCache.GetOrAdd(objectType, type =>
		{
			var autoInjectProperties = new List<Tuple<PropertyInfo, Type, bool>>();
			var propertiesOfType = type.GetRuntimeProperties().ToList();

			foreach (var property in propertiesOfType)
			{
				if (property.GetCustomAttribute<InjectAttribute>() == null)
				{
					continue;
				}

				var (propertyType, multiple) = FindServiceType(property.Name, property.PropertyType);

				autoInjectProperties.Add(Tuple.Create(property, propertyType, multiple));
			}

			return autoInjectProperties;
		});
	}

	internal static MethodInfo FindFactoryMethod<TTarget, TAttribute>(object[] criteria)
		where TAttribute : FactoryMethodAttribute
	{
		return FindFactoryMethod<TTarget>(typeof(TAttribute), criteria);
	}

	internal static MethodInfo FindFactoryMethod<TTarget>(Type attributeType, object[] criteria)
	{
		// Type[] types;
		// if (criteria != null)
		// {
		//     types = new Type[criteria.Length];
		//     for (var index = 0; index < criteria.Length; index++)
		//     {
		//         types[index] = criteria[index].GetType();
		//     }
		// }
		// else
		// {
		//     types = null;
		// }
		//
		var name = GetMethodName<TTarget>(attributeType, criteria);
		return _factoryMethods.GetOrAdd(name, () => FindMatchedMethod<TTarget>(attributeType, criteria));
	}

	private static MethodInfo FindMatchedMethod<TTarget>(Type attributeType, object[] criteria)
	{
		var candidates = GetCandidateMethods(typeof(TTarget), attributeType);
		if (candidates == null || candidates.Count == 0)
		{
			throw new MissingMethodException(typeof(TTarget).FullName, GetConventionalMethodNames(attributeType).JoinAsString("/"));
		}

		var matches = new List<Tuple<MethodInfo, int>>();

		int parameterCount;
		if (criteria != null)
		{
			if (criteria.GetType() == typeof(object[]))
			{
				parameterCount = criteria.GetLength(0);
			}
			else
			{
				parameterCount = 1;
			}
		}
		else
		{
			parameterCount = 1;
		}

		if (parameterCount > 0)
		{
			foreach (var candidate in candidates)
			{
				var score = 0;
				var methodParameters = candidate.Item1.GetParameters();
				if (methodParameters.Length != parameterCount)
				{
					continue;
				}

				var index = 0;

				if (criteria!.GetType() == typeof(object[]))
				{
					foreach (var c in criteria)
					{
						var currentScore = CalculateParameterMatchScore(methodParameters[index], c);
						if (currentScore == 0)
						{
							break;
						}

						score += currentScore;
						index++;
					}
				}
				else
				{
					var currentScore = CalculateParameterMatchScore(methodParameters[index], criteria);
					if (currentScore != 0)
					{
						score += currentScore;
						index++;
					}
				}

				if (index == parameterCount)
				{
					matches.Add(Tuple.Create(candidate.Item1, score + candidate.Item2));
				}
			}
		}
		else
		{
			foreach (var (method, score) in candidates)
			{
				if (method.GetParameters().Length == 0)
				{
					matches.Add(Tuple.Create(method, score));
				}
			}
		}

		if (matches.Count == 0)
		{
			// look for params array
			foreach (var (method, score) in candidates)
			{
				var lastParam = method.GetParameters().LastOrDefault();
				if (lastParam != null && lastParam.ParameterType == typeof(object[]) &&
					lastParam.GetCustomAttributes<ParamArrayAttribute>().Any())
				{
					matches.Add(Tuple.Create(method, 1 + score));
				}
			}
		}

		if (matches.Count == 0)
		{
			var methodNames = GetConventionalMethodNames(attributeType).JoinAsString("/");
			var paramsNames = GetParameterTypeNames(criteria);
			throw new MissingMethodException(typeof(TTarget).FullName, $"{methodNames}({paramsNames})");
		}

		{
			var method = matches[0];
			if (matches.Count > 1)
			{
				var maxScore = int.MinValue;
				var maxCount = 0;
				foreach (var item in matches)
				{
					if (item.Item2 > maxScore)
					{
						maxScore = item.Item2;
						maxCount = 1;
						method = item;
					}
					else if (item.Item2 == maxScore)
					{
						maxCount++;
					}
				}

				if (maxCount > 1)
				{
					throw new AmbiguousMatchException(Resources.IDS_MULTIPLE_METHOD_MATCHED);
				}
			}

			return method.Item1;
		}
	}

	/// <summary>
	/// Find matched method.
	/// </summary>
	/// <param name="attributeType"></param>
	/// <param name="parameterTypes"></param>
	/// <typeparam name="TTarget"></typeparam>
	/// <returns></returns>
	/// <exception cref="MissingMethodException"></exception>
	/// <exception cref="AmbiguousMatchException"></exception>
	public static MethodInfo FindMatchedMethod<TTarget>(Type attributeType, IReadOnlyList<Type> parameterTypes)
	{
		var methods = typeof(TTarget).GetRuntimeMethods()
									 .Where(t => t.GetCustomAttribute(attributeType) != null);
		if (methods == null || !methods.Any())
		{
			throw new MissingMethodException($"Missing method with attribute '{attributeType.Name}' on {typeof(TTarget).FullName}");
		}

		var matches = new List<MethodInfo>();

		foreach (var method in methods)
		{
			var parameters = method.GetParameters();

			if (parameters.Length == 0 && parameterTypes.Count == 0)
			{
				matches.Add(method);
				continue;
			}

			if (parameters.Length != parameterTypes.Count)
			{
				continue;
			}

			var isAllParameterMatched = Enumerable.Range(0, parameters.Length).Select(index => parameters[index].ParameterType == parameterTypes[index]).All(t => t);
			if (isAllParameterMatched)
			{
				matches.Add(method);
			}
		}

		return matches.Count switch
		{
			0 => throw new MissingMethodException("Missing method matched the specified arguments."),
			1 => matches[0],
			_ => throw new AmbiguousMatchException("Multiple methods matched.")
		};
	}

	private static List<Tuple<MethodInfo, int>> GetCandidateMethods(Type targetType, Type attributeType, int level = 0)
	{
		var validNames = GetConventionalMethodNames(attributeType);

		var result = new List<Tuple<MethodInfo, int>>();
		var methods = targetType.GetMethods(BINDING_FLAGS)
								.Where(t => t.GetCustomAttribute(attributeType) != null || validNames.Contains(t.Name));

		// ReSharper disable once LoopCanBeConvertedToQuery
		foreach (var method in methods)
		{
			result.Add(Tuple.Create(method, level));
		}

		if (result.Count == 0 && targetType.BaseType != null && targetType.BaseType != typeof(object) && !targetType.BaseType.IsInterface)
		{
			level--;
			result.AddRange(GetCandidateMethods(targetType.BaseType, attributeType, level));
		}

		{
		}

		return result;
	}

	/// <summary>
	/// Find injected service type for property type.
	/// </summary>
	/// <param name="name">The property name.</param>
	/// <param name="type">The property type.</param>
	/// <param name="multiple">If the service has mutiple implements.</param>
	/// <returns></returns>
	/// <exception cref="NotSupportedException"></exception>
	/// <exception cref="InvalidOperationException"></exception>
	private static Tuple<Type, bool> FindServiceType(string name, Type type, bool? multiple = null)
	{
		if (type.IsPrimitive)
		{
			throw new NotSupportedException("Can not inject primitive type property.");
		}

		if (!type.IsClass && !type.IsInterface)
		{
			throw new NotSupportedException($"Can not inject property '{name}', the property type {type.FullName} is not supported.");
		}

		if (type == typeof(object))
		{
			throw new NotSupportedException($"Can not inject property '{name}', the property type {type.FullName} is not supported.");
		}

		var @interface = type.GetInterface("IEnumerable");
		if (@interface == null)
		{
			return Tuple.Create(type, multiple ?? false);
		}

		if (multiple == true)
		{
			throw new NotSupportedException();
		}

		if (type.IsArray)
		{
			var interfaces = type.FindInterfaces(HandlerInterfaceFilter, null);
			if (interfaces == null || interfaces.Length == 0)
			{
				throw new InvalidOperationException();
			}

			return FindServiceType(name, interfaces[0].GenericTypeArguments[0], true);
		}

		if (type.IsGenericType)
		{
			var propertyTypeFullname = $"{type.Namespace}.{type.Name}";
			if (propertyTypeFullname == typeof(IEnumerable<>).FullName)
			{
				if (type.GenericTypeArguments.Length != 1)
				{
					throw new InvalidOperationException("");
				}

				var genericArgumentType = type.GenericTypeArguments[0];

				return FindServiceType(name, genericArgumentType, true);
			}
		}

		throw new NotSupportedException($"Can not inject property '{name}', the property type {type.FullName} is not supported.");
	}

	private static bool HandlerInterfaceFilter(Type type, object criteria)
	{
		var typeName = $"{type.Namespace}.{type.Name}";
		return _collectionTypesName.Contains(typeName);
	}

	private static string GetMethodName<TTarget>(Type attributeType, object[] criteria)
	{
		var name = $"{typeof(TTarget).FullName}.{attributeType.Name.Replace(nameof(Attribute), string.Empty)}({GetParameterTypeNames(criteria)})";
		return name;
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="criteria"></param>
	/// <returns></returns>
	private static string GetParameterTypeNames(object[] criteria)
	{
		if (criteria == null)
		{
			return string.Empty;
		}

		var parameterTypeNames = new List<string>();
		if (criteria.GetType() == typeof(object[]))
		{
			// ReSharper disable once LoopCanBeConvertedToQuery
			foreach (var item in criteria)
			{
				parameterTypeNames.Add(item == null ? "null" : GetTypeName(item.GetType()));
			}
		}
		else
		{
			parameterTypeNames.Add(GetTypeName(criteria.GetType()));
		}

		return parameterTypeNames.Join(", ");
	}

	private static string GetTypeName(Type type)
	{
		if (type.IsArray)
		{
			return $"{GetTypeName(type.GetElementType())}[]";
		}

		if (!type.IsGenericType)
		{
			return type.Name;
		}

		var result = new StringBuilder();
		var genericArguments = type.GetGenericArguments();
		result.Append(type.Name);
		result.Append('<');

		for (var index = 0; index < genericArguments.Length; index++)
		{
			if (index > 0)
			{
				result.Append(',');
			}

			result.Append(GetTypeName(genericArguments[index]));
		}

		result.Append('>');

		return result.ToString();
	}

	/// <summary>
	/// Calculate parameter match score.
	/// </summary>
	private static int CalculateParameterMatchScore(ParameterInfo parameter, object criteria)
	{
		if (criteria == null)
		{
			if (parameter.ParameterType.IsPrimitive)
			{
				return 0;
			}

			if (parameter.ParameterType == typeof(object))
			{
				return 2;
			}

			if (parameter.ParameterType == typeof(object[]))
			{
				return 2;
			}

			if (parameter.ParameterType.IsClass)
			{
				return 1;
			}

			if (parameter.ParameterType.IsArray)
			{
				return 1;
			}

			if (parameter.ParameterType.IsInterface)
			{
				return 1;
			}

			if (Nullable.GetUnderlyingType(parameter.ParameterType) != null)
			{
				return 2;
			}
		}
		else
		{
			if (criteria.GetType() == parameter.ParameterType)
			{
				return 3;
			}

			if (parameter.ParameterType == typeof(object))
			{
				return 1;
			}

			if (parameter.ParameterType.IsInstanceOfType(criteria))
			{
				return 2;
			}
		}

		return 0;
	}

	/// <summary>
	/// Get conventional method names.
	/// </summary>
	/// <param name="attributeType"></param>
	/// <returns></returns>
	private static string[] GetConventionalMethodNames(Type attributeType)
	{
		var validNames = new[]
			{
			$"Factory{attributeType.Name.Replace(nameof(Attribute), string.Empty)}",
			$"Factory{attributeType.Name.Replace(nameof(Attribute), string.Empty)}Async",
			$"{attributeType.Name.Replace(nameof(Attribute), string.Empty)}",
			$"{attributeType.Name.Replace(nameof(Attribute), string.Empty)}Async"
		};
		return validNames;
	}
}