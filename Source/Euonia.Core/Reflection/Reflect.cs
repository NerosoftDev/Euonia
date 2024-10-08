﻿using System.Linq.Expressions;
using System.Reflection;

namespace Nerosoft.Euonia.Reflection;

/// <summary>
/// Provides strong-typed reflection
/// </summary>
public static class Reflect
{
	/// <summary>
	/// Extracts the property from a property expression.
	/// </summary>
	/// <typeparam name="T">The object type containing the property specified in the expression.</typeparam>
	/// <param name="expression">The property expression (e.g. p =&gt; p.PropertyName)</param>
	/// <returns>The name of the property.</returns>
	/// <exception cref="ArgumentNullException">Thrown if the <paramref name="expression" /> is null.</exception>
	/// <exception cref="ArgumentException">Thrown when the expression is:<br />
	/// Not a <see cref="MemberExpression" /><br />
	/// The <see cref="MemberExpression" /> does not represent a property.<br />
	/// Or, the property is static.</exception>
	public static PropertyInfo GetProperty<T>(Expression<Func<T>> expression)
	{
		ArgumentAssert.ThrowIfNull(expression, nameof(expression));

		if (expression.Body is not MemberExpression memberExpression)
		{
			throw new ArgumentException(Resources.IDS_EXPRESSION_IS_NOT_A_MEMBER_ACCESS_EXPRESSION, nameof(expression));
		}

		var property = memberExpression.Member as PropertyInfo;
		if (property == null)
		{
			throw new ArgumentException(Resources.IDS_MEMBER_ACCESS_EXPRESSION_DOES_NOT_ACCESS_A_PROPERTY, nameof(expression));
		}

		return property;
	}

	/// <summary>
	/// Extracts the property from a property expression.
	/// </summary>
	/// <param name="expression"></param>
	/// <typeparam name="T"></typeparam>
	/// <returns></returns>
	public static PropertyInfo GetProperty<T>(Expression<Func<T, object>> expression)
	{
		return GetProperty<T, object>(expression);
	}

	/// <summary>
	/// Extracts the property from a property expression.
	/// </summary>
	/// <param name="expression"></param>
	/// <typeparam name="T"></typeparam>
	/// <typeparam name="TResult"></typeparam>
	/// <returns></returns>
	/// <exception cref="ArgumentNullException"></exception>
	/// <exception cref="ArgumentException"></exception>
	public static PropertyInfo GetProperty<T, TResult>(Expression<Func<T, TResult>> expression)
	{
		ArgumentAssert.ThrowIfNull(expression, nameof(expression));

		PropertyInfo result;

		if (expression.Body.NodeType == ExpressionType.Convert)
		{
			result = ((MemberExpression)((UnaryExpression)expression.Body).Operand).Member as PropertyInfo;
		}
		else
		{
			result = ((MemberExpression)expression.Body).Member as PropertyInfo;
		}

		if (result != null)
		{
			return result;
		}

		throw new ArgumentException($"Expression '{expression}' does not refer to a property.");
	}

	/// <summary>
	/// Gets the method information represented by the lambda expression.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="expression"></param>
	/// <returns></returns>
	public static MethodInfo GetMethodInfo<T>(Expression<Func<T, Delegate>> expression)
	{
		return GetMethodInfo((LambdaExpression)expression);
	}

	//public static MethodInfo GetMethodInfo(LambdaExpression expression)
	//{
	//    var unaryExpression = (UnaryExpression)expression.Body;
	//    var methodCallExpression = (MethodCallExpression)unaryExpression.Operand;
	//    var methodCallObject = (ConstantExpression)methodCallExpression.Object;
	//    var methodInfo = (MethodInfo)methodCallObject.Value;
	//    return methodInfo;
	//}

	/// <summary>
	/// Gets the method information represented by the lambda expression.
	/// </summary>
	/// <param name="expression"></param>
	/// <returns></returns>
	/// <exception cref="ArgumentNullException"></exception>
	/// <exception cref="ArgumentException"></exception>
	/// <exception cref="NullReferenceException"></exception>
	public static MethodInfo GetMethodInfo(Expression expression)
	{
		if (expression == null)
		{
			throw new ArgumentNullException(nameof(expression));
		}

		if (expression is not LambdaExpression lambda)
		{
			throw new ArgumentException(Resources.IDS_NOT_A_LAMBDA_EXPRESSION, nameof(expression));
		}

		// ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
		switch (lambda.Body.NodeType)
		{
			case ExpressionType.Convert:
			{
				var unaryExpression = (UnaryExpression)lambda.Body;
				var methodCallExpression = (MethodCallExpression)unaryExpression.Operand;
				var methodCallObject = (ConstantExpression)methodCallExpression.Object;
				if (methodCallObject == null)
				{
					throw new NullReferenceException(nameof(methodCallObject));
				}

				var methodInfo = (MethodInfo)methodCallObject.Value;
				return methodInfo;
			}
			case ExpressionType.Call:
				return ((MethodCallExpression)lambda.Body).Method;
			default:
				throw new ArgumentException(Resources.IDS_NOT_A_METHOD_CALL, nameof(expression));
		}
	}

	/// <summary>
	/// Get the member information represented by the lambda expression.
	/// </summary>
	/// <param name="expression"></param>
	/// <returns></returns>
	/// <exception cref="ArgumentNullException"></exception>
	/// <exception cref="ArgumentException"></exception>
	public static MemberInfo GetMemberInfo(Expression expression)
	{
		if (expression == null)
		{
			throw new ArgumentNullException(nameof(expression));
		}

		if (expression is not LambdaExpression lambda)
		{
			throw new ArgumentException(Resources.IDS_NOT_A_LAMBDA_EXPRESSION, nameof(expression));
		}

		var memberExpr = lambda.Body.NodeType switch
		{
			// The Func<TTarget, object> we use returns an object, so first statement can be either 
			// a cast (if the field/property does not return an object) or the direct member access.
			ExpressionType.Convert => ((UnaryExpression)lambda.Body).Operand as MemberExpression,
			ExpressionType.MemberAccess => lambda.Body as MemberExpression,
			_ => null
		};

		if (memberExpr == null)
		{
			throw new ArgumentException(Resources.IDS_NOT_A_MEMBER_ACCESS, nameof(expression));
		}

		return memberExpr.Member;
	}

	/// <summary>
	/// Sets the value to property.
	/// </summary>
	/// <typeparam name="T">The item type.</typeparam>
	/// <typeparam name="TValue">The type of the value.</typeparam>
	/// <param name="item">The item.</param>
	/// <param name="value">The value.</param>
	/// <param name="property">The property.</param>
	/// <exception cref="ArgumentNullException">property</exception>
	public static void SetValue<T, TValue>(T item, TValue value, Expression<Func<T, TValue>> property)
	{
		if (property == null)
		{
			throw new ArgumentNullException(nameof(property));
		}

		var propertyInfo = GetProperty(property);

		propertyInfo.SetValue(item, value);
	}

	/// <summary>
	/// Gets the value.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="item">The item.</param>
	/// <param name="property">The property.</param>
	/// <returns>System.Object.</returns>
	/// <exception cref="ArgumentNullException">property</exception>
	public static object GetValue<T>(T item, Expression<Func<T, object>> property)
	{
		if (property == null)
		{
			throw new ArgumentNullException(nameof(property));
		}

		var propertyInfo = GetProperty(property);

		return propertyInfo.GetValue(item);
	}

	/// <summary>
	/// Checks whether <paramref name="givenType"/> implements/inherits <paramref name="genericType"/>.
	/// </summary>
	/// <param name="givenType">Type to check</param>
	/// <param name="genericType">Generic type</param>
	public static bool IsAssignableToGenericType(Type givenType, Type genericType)
	{
		var givenTypeInfo = givenType.GetTypeInfo();

		if (givenTypeInfo.IsGenericType && givenType.GetGenericTypeDefinition() == genericType)
		{
			return true;
		}

		foreach (var interfaceType in givenTypeInfo.GetInterfaces())
		{
			if (interfaceType.GetTypeInfo().IsGenericType && interfaceType.GetGenericTypeDefinition() == genericType)
			{
				return true;
			}
		}

		return givenTypeInfo.BaseType != null && IsAssignableToGenericType(givenTypeInfo.BaseType, genericType);
	}

	/// <summary>
	/// Gets the implemented generic types.
	/// </summary>
	/// <param name="givenType"></param>
	/// <param name="genericType"></param>
	/// <returns></returns>
	public static List<Type> GetImplementedGenericTypes(Type givenType, Type genericType)
	{
		var result = new List<Type>();
		AddImplementedGenericTypes(result, givenType, genericType);
		return result;
	}

	private static void AddImplementedGenericTypes(ICollection<Type> result, Type givenType, Type genericType)
	{
		var givenTypeInfo = givenType.GetTypeInfo();

		if (givenTypeInfo.IsGenericType && givenType.GetGenericTypeDefinition() == genericType)
		{
			result.AddIfNotContains(givenType);
		}

		foreach (var interfaceType in givenTypeInfo.GetInterfaces())
		{
			if (interfaceType.GetTypeInfo().IsGenericType && interfaceType.GetGenericTypeDefinition() == genericType)
			{
				result.AddIfNotContains(interfaceType);
			}
		}

		if (givenTypeInfo.BaseType == null)
		{
			return;
		}

		AddImplementedGenericTypes(result, givenTypeInfo.BaseType, genericType);
	}

	/// <summary>
	/// Tries to gets an of attribute defined for a class member and it's declaring type including inherited attributes.
	/// Returns default value if it's not declared at all.
	/// </summary>
	/// <typeparam name="TAttribute">Type of the attribute</typeparam>
	/// <param name="memberInfo">MemberInfo</param>
	/// <param name="defaultValue">Default value (null as default)</param>
	/// <param name="inherit">Inherit attribute from base classes</param>
	public static TAttribute GetSingleAttributeOrDefault<TAttribute>(MemberInfo memberInfo, TAttribute defaultValue = default, bool inherit = true)
		where TAttribute : Attribute
	{
		//Get attribute on the member
		if (memberInfo.IsDefined(typeof(TAttribute), inherit))
		{
			return memberInfo.GetCustomAttributes(typeof(TAttribute), inherit).Cast<TAttribute>().First();
		}

		return defaultValue;
	}

	/// <summary>
	/// Tries to gets an of attribute defined for a class member and it's declaring type including inherited attributes.
	/// Returns default value if it's not declared at all.
	/// </summary>
	/// <typeparam name="TAttribute">Type of the attribute</typeparam>
	/// <param name="memberInfo">MemberInfo</param>
	/// <param name="defaultValue">Default value (null as default)</param>
	/// <param name="inherit">Inherit attribute from base classes</param>
	public static TAttribute GetSingleAttributeOfMemberOrDeclaringTypeOrDefault<TAttribute>(MemberInfo memberInfo, TAttribute defaultValue = default, bool inherit = true)
		where TAttribute : class
	{
		return memberInfo.GetCustomAttributes(inherit).OfType<TAttribute>().FirstOrDefault()
		       ?? memberInfo.DeclaringType?.GetTypeInfo().GetCustomAttributes(true).OfType<TAttribute>().FirstOrDefault()
		       ?? defaultValue;
	}

	/// <summary>
	/// Tries to gets attributes defined for a class member and it's declaring type including inherited attributes.
	/// </summary>
	/// <typeparam name="TAttribute">Type of the attribute</typeparam>
	/// <param name="memberInfo">MemberInfo</param>
	/// <param name="inherit">Inherit attribute from base classes</param>
	public static IEnumerable<TAttribute> GetAttributesOfMemberOrDeclaringType<TAttribute>(MemberInfo memberInfo, bool inherit = true)
		where TAttribute : class
	{
		var customAttributes = memberInfo.GetCustomAttributes(inherit).OfType<TAttribute>();
		var declaringTypeCustomAttributes =
			memberInfo.DeclaringType?.GetTypeInfo().GetCustomAttributes(true).OfType<TAttribute>();
		return declaringTypeCustomAttributes != null
			? customAttributes.Concat(declaringTypeCustomAttributes).Distinct()
			: customAttributes;
	}

	/// <summary>
	/// Gets value of a property by it's full path from given object
	/// </summary>
	public static object GetValue(object obj, Type objectType, string propertyPath)
	{
		var value = obj;
		var currentType = objectType;
		var objectPath = currentType.FullName;
		var absolutePropertyPath = propertyPath;
		if (objectPath != null && absolutePropertyPath.StartsWith(objectPath))
		{
			absolutePropertyPath = absolutePropertyPath.Replace(objectPath + ".", "");
		}

		foreach (var propertyName in absolutePropertyPath.Split('.'))
		{
			var property = currentType.GetProperty(propertyName);
			if (property != null)
			{
				if (value != null)
				{
					value = property.GetValue(value, null);
				}

				currentType = property.PropertyType;
			}
			else
			{
				value = null;
				break;
			}
		}

		return value;
	}

	/// <summary>
	/// Sets value of a property by it's full path on given object
	/// </summary>
	public static void SetValue(object obj, Type objectType, string propertyPath, object value)
	{
		var currentType = objectType;
		PropertyInfo property;
		var objectPath = currentType.FullName;
		var absolutePropertyPath = propertyPath;
		if (absolutePropertyPath.StartsWith(objectPath!))
		{
			absolutePropertyPath = absolutePropertyPath.Replace(objectPath + ".", "");
		}

		var properties = absolutePropertyPath.Split('.');

		if (properties.Length == 1)
		{
			property = objectType.GetProperty(properties.First());
			if (property == null)
			{
				throw new MissingMemberException($"Property {properties.First()} not found on type {objectType.FullName}.");
			}

			property.SetValue(obj, value);
			return;
		}

		for (var i = 0; i < properties.Length - 1; i++)
		{
			property = currentType.GetProperty(properties[i]);
			if (property == null)
			{
				throw new MissingMemberException($"Property {properties[i]} not found on type {currentType.FullName}.");
			}

			obj = property.GetValue(obj, null);
			currentType = property.PropertyType;
		}

		property = currentType.GetProperty(properties.Last());
		if (property == null)
		{
			throw new MissingMemberException($"Property {properties.Last()} not found on type {currentType.FullName}.");
		}

		property.SetValue(obj, value);
	}

	/// <summary>
	/// Get all the constant values in the specified type (including the base type).
	/// </summary>
	/// <param name="type"></param>
	/// <returns></returns>
	public static IEnumerable<string> GetPublicConstantsRecursively(Type type)
	{
		const int maxRecursiveParameterValidationDepth = 8;

		var publicConstants = new List<string>();

		static void Recursively(List<string> constants, Type targetType, int currentDepth)
		{
			if (currentDepth > maxRecursiveParameterValidationDepth)
			{
				return;
			}

			constants.AddRange(targetType.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
			                             .Where(x => x.IsLiteral && !x.IsInitOnly)
			                             .Select(x => x.GetValue(null)?.ToString()));

			var nestedTypes = targetType.GetNestedTypes(BindingFlags.Public);

			foreach (var nestedType in nestedTypes)
			{
				Recursively(constants, nestedType, currentDepth + 1);
			}
		}

		Recursively(publicConstants, type, 1);

		return publicConstants.ToArray();
	}

	/// <summary>
	/// Invokes the generic method.
	/// </summary>
	/// <param name="obj"></param>
	/// <param name="methodName"></param>
	/// <param name="genericTypes"></param>
	/// <param name="parameters"></param>
	/// <returns></returns>
	/// <exception cref="ArgumentNullException"></exception>
	public static object InvokeGenericMethod(object obj, string methodName, Type[] genericTypes, params object[] parameters)
	{
		var method = obj.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public);
		if (method == null)
		{
			throw new ArgumentNullException($"Method {methodName} not found");
		}

		var genericMethod = method.MakeGenericMethod(genericTypes);
		return genericMethod.Invoke(method.IsStatic ? null : obj, parameters);
	}

	#region Public Static Methods

	/// <summary>
	/// Attempts to find the overloaded method that we want to call. Returns null if not found. This overload looks at the parameter types passed in vs method parameters off of the type we pass in
	/// </summary>
	/// <param name="methodNameToRetrieve">What is the method to name to find</param>
	/// <param name="typeToLookThroughTheMethods">The type to retrieve the methods off of, so we can look through it and try to find the correct method</param>
	/// <param name="methodParameterTypes">Look for the method parameter types in the method to match. If the method takes a string and an int, then we will look for that in every method</param>
	/// <returns>Method info found, or null value if not found</returns>
	public static MethodInfo FindMethod(string methodNameToRetrieve, Type typeToLookThroughTheMethods, params Type[] methodParameterTypes)
	{
		//going to use the overload. So we can create the func with calling the other method
		return FindMethod(methodNameToRetrieve, typeToLookThroughTheMethods, x => MethodParameterSelector(x, methodParameterTypes));
	}

	/// <summary>
	/// Attempts to find the overloaded method that we want to call. Returns null if not found. This method will try to evaluate the MethodSelect for each method and check to see if it returns true.
	/// </summary>
	/// <param name="methodNameToRetrieve">What is the method to name to find</param>
	/// <param name="methodSelector">Gives the calling method the ability to look through the parameters and pick the correct method</param>
	/// <param name="typeToLookThroughTheMethods">The type to retrieve the methods off of, so we can look through it and try to find the correct method</param>
	/// <returns>Method info found, or null value if not found</returns>
	public static MethodInfo FindMethod(string methodNameToRetrieve, Type typeToLookThroughTheMethods, Func<MethodInfo, bool> methodSelector)
	{
		//use the overload
		return FindMethod(methodNameToRetrieve, typeToLookThroughTheMethods.GetMethods(), methodSelector);
	}

	/// <summary>
	/// Attempts to find the overloaded method that we want to call. Returns null if not found. This method will try to evaluate the MethodSelect for each method and check to see if it returns true.
	/// Call this method if you already have the method info's that match the same name you are looking for
	/// </summary>
	/// <param name="methodNameToRetrieve">What is the method to name to find</param>
	/// <param name="methodSelector">Gives the calling method the ability to look through the parameters and pick the correct method</param>
	/// <param name="methodsToLookThrough">Methods that have the same name. Or methods to loop through and inspect against the method selector.</param>
	/// <returns>Method info found, or null value if not found</returns>
	public static MethodInfo FindMethod(string methodNameToRetrieve, IEnumerable<MethodInfo> methodsToLookThrough, Func<MethodInfo, bool> methodSelector)
	{
		//let's start looping through the methods to see if we can find a match
		return methodsToLookThrough.FirstOrDefault(methodToInspect => string.Equals(methodNameToRetrieve, methodToInspect.Name, StringComparison.OrdinalIgnoreCase) && methodSelector(methodToInspect));

		//we never found a match, so just return null
	}

	#endregion

	#region Private Static Methods

	/// <summary>
	/// Private helper method to look at the current method and inspect it for the method parameter types. If they match return true, else return false
	/// </summary>
	/// <param name="methodToEvaluate">Method to evaluate and check if we have a match based on the method parameter types</param>
	/// <param name="methodParameterTypes"></param>
	/// <returns>Do we have a match? Do the method parameter types match?</returns>
	private static bool MethodParameterSelector(MethodBase methodToEvaluate, params Type[] methodParameterTypes)
	{
		//we are going to match the GetParameters and the MethodParameterTypes. It needs to match index for index and type for type. So GetParameters[0].Type must match MethodParameterTypes[0].Type...[1].Type must match [1].Type

		//holds the index with the method parameter types we are up too
		int i = 0;

		//let's loop through the parameters
		foreach (ParameterInfo thisParameter in methodToEvaluate.GetParameters())
		{
			//it's a generic parameter...ie...TSource then we are going to ignore it because whatever we pass in would be TSource
			if (!thisParameter.ParameterType.IsGenericParameter)
			{
				//is this a generic type? we need to compare this differently
				if (thisParameter.ParameterType.IsGenericType)
				{
					//is the method parameter a generic type?
					if (!methodParameterTypes[i].IsGenericType)
					{
						//it isn't so return false..cause they aren't the same
						return false;
					}

					//if the generic type's don't match then return false...This might be problematic...it works for the scenario which I'm using it for so we will leave this and modify afterwards
					if (thisParameter.ParameterType.GetGenericTypeDefinition() != methodParameterTypes[i].GetGenericTypeDefinition())
					{
						//doesn't match return false
						return false;
					}
				}
				else if (thisParameter.ParameterType != methodParameterTypes[i].UnderlyingSystemType)
				{
					//this is a regular parameter so we can compare it normally
					//we don't have a match...so return false
					return false;
				}
			}

			//increment the index
			i++;
		}

		//if we get here then everything matches so return true
		return true;
	}

	#endregion
}

/// <summary>
/// Provides strong-typed reflection of the <typeparamref name="TTarget"/> 
/// type.
/// </summary>
/// <typeparam name="TTarget">Type to reflect.</typeparam>
public static class Reflect<TTarget>
{
	/// <summary>
	/// Gets the method represented by the lambda expression.
	/// </summary>
	/// <exception cref="ArgumentNullException">The <paramref name="expression"/> is null.</exception>
	/// <exception cref="ArgumentException">The <paramref name="expression"/> is not a lambda expression or it does not represent a method invocation.</exception>
	public static MethodInfo GetMethod(Expression<Action<TTarget>> expression)
	{
		return Reflect.GetMethodInfo(expression);
	}

	/// <summary>
	/// Gets the method represented by the lambda expression.
	/// </summary>
	/// <exception cref="ArgumentNullException">The <paramref name="expression"/> is null.</exception>
	/// <exception cref="ArgumentException">The <paramref name="expression"/> is not a lambda expression or it does not represent a method invocation.</exception>
	public static MethodInfo GetMethod<T1>(Expression<Action<TTarget, T1>> expression)
	{
		return Reflect.GetMethodInfo(expression);
	}

	/// <summary>
	/// Gets the method represented by the lambda expression.
	/// </summary>
	/// <exception cref="ArgumentNullException">The <paramref name="expression"/> is null.</exception>
	/// <exception cref="ArgumentException">The <paramref name="expression"/> is not a lambda expression or it does not represent a method invocation.</exception>
	public static MethodInfo GetMethod<T1, T2>(Expression<Action<TTarget, T1, T2>> expression)
	{
		return Reflect.GetMethodInfo(expression);
	}

	/// <summary>
	/// Gets the method represented by the lambda expression.
	/// </summary>
	/// <exception cref="ArgumentNullException">The <paramref name="expression"/> is null.</exception>
	/// <exception cref="ArgumentException">The <paramref name="expression"/> is not a lambda expression or it does not represent a method invocation.</exception>
	public static MethodInfo GetMethod<T1, T2, T3>(Expression<Action<TTarget, T1, T2, T3>> expression)
	{
		return Reflect.GetMethodInfo(expression);
	}

	/// <summary>
	/// Gets the property represented by the lambda expression.
	/// </summary>
	/// <exception cref="ArgumentNullException">The <paramref name="expression"/> is null.</exception>
	/// <exception cref="ArgumentException">The <paramref name="expression"/> is not a lambda expression or it does not represent a property access.</exception>
	public static PropertyInfo GetProperty(Expression<Func<TTarget, object>> expression)
	{
		var info = Reflect.GetMemberInfo(expression) as PropertyInfo;
		if (info == null)
			throw new ArgumentException("Member is not a property");

		return info;
	}

	/// <summary>
	/// Gets the property represented by the lambda expression.
	/// </summary>
	/// <typeparam name="TValue">Type assigned to the property</typeparam>
	/// <param name="expression">Property Expression</param>
	/// <returns></returns>
	/// <exception cref="ArgumentNullException">The <paramref name="expression"/> is null.</exception>
	/// <exception cref="ArgumentException">The <paramref name="expression"/> is not a lambda expression or it does not represent a property access.</exception>
	public static PropertyInfo GetProperty<TValue>(Expression<Func<TTarget, TValue>> expression)
	{
		var info = Reflect.GetMemberInfo(expression) as PropertyInfo;
		if (info == null)
		{
			throw new ArgumentException("Member is not a property");
		}

		return info;
	}

	/// <summary>
	/// Gets the field represented by the lambda expression.
	/// </summary>
	/// <exception cref="ArgumentNullException">The <paramref name="expression"/> is null.</exception>
	/// <exception cref="ArgumentException">The <paramref name="expression"/> is not a lambda expression or it does not represent a field access.</exception>
	public static FieldInfo GetField(Expression<Func<TTarget, object>> expression)
	{
		var info = Reflect.GetMemberInfo(expression) as FieldInfo;
		if (info == null)
		{
			throw new ArgumentException("Member is not a field");
		}

		return info;
	}

	/// <summary>
	/// Sets the value to property.
	/// </summary>
	/// <typeparam name="TValue">The type of the value.</typeparam>
	/// <param name="item">The item.</param>
	/// <param name="value">The value.</param>
	/// <param name="property">The property.</param>
	/// <exception cref="ArgumentNullException">property</exception>
	public static void SetValue<TValue>(TTarget item, TValue value, Expression<Func<TTarget, TValue>> property)
	{
		ArgumentAssert.ThrowIfNull(property, nameof(property));

		var propertyInfo = GetProperty(property);

		propertyInfo.SetValue(item, value);
	}

	/// <summary>
	/// Gets the value.
	/// </summary>
	/// <param name="item">The item.</param>
	/// <param name="property">The property.</param>
	/// <returns>System.Object.</returns>
	/// <exception cref="ArgumentNullException">property</exception>
	public static object GetValue(TTarget item, Expression<Func<TTarget, object>> property)
	{
		if (property == null)
		{
			throw new ArgumentNullException(nameof(property));
		}

		var propertyInfo = GetProperty(property);

		return propertyInfo.GetValue(item);
	}

	/// <summary>
	/// Detects whether the specified type is assignable to a generic type.
	/// </summary>
	/// <param name="genericType"></param>
	/// <returns></returns>
	public static bool IsAssignableToGenericType(Type genericType)
	{
		return Reflect.IsAssignableToGenericType(typeof(TTarget), genericType);
	}

	/// <summary>
	/// Gets the implemented generic types.
	/// </summary>
	/// <param name="genericType"></param>
	/// <returns></returns>
	public static List<Type> GetImplementedGenericTypes(Type genericType)
	{
		return Reflect.GetImplementedGenericTypes(typeof(TTarget), genericType);
	}

	/// <summary>
	/// Gets value of the property.
	/// </summary>
	/// <param name="obj"></param>
	/// <param name="propertyPath"></param>
	/// <returns></returns>
	public static object GetValue(TTarget obj, string propertyPath)
	{
		return Reflect.GetValue(obj, typeof(TTarget), propertyPath);
	}

	/// <summary>
	/// Sets value of the property.
	/// </summary>
	/// <param name="obj"></param>
	/// <param name="propertyPath"></param>
	/// <param name="value"></param>
	public static void SetValue(TTarget obj, string propertyPath, object value)
	{
		Reflect.SetValue(obj, typeof(TTarget), propertyPath, value);
	}

	/// <summary>
	/// Gets the public constants recursively.
	/// </summary>
	/// <returns></returns>
	public static IEnumerable<string> GetPublicConstantsRecursively()
	{
		return Reflect.GetPublicConstantsRecursively(typeof(TTarget));
	}
}