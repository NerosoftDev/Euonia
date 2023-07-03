using System.Diagnostics.CodeAnalysis;
using System.Reflection;

public static partial class Extensions
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static string GetFullNameWithAssemblyName(this Type type)
    {
        return type.FullName + ", " + type.Assembly.GetName().Name;
    }

    /// <summary>
    /// Determines whether an instance of this type can be assigned to
    /// an instance of the <typeparamref name="TTarget"></typeparamref>.
    /// Internally uses <see cref="Type.IsAssignableFrom"/>.
    /// </summary>
    /// <typeparam name="TTarget">Target type</typeparam> (as reverse).
    public static bool IsAssignableTo<TTarget>([NotNull] this Type type)
    {
        Check.EnsureNotNull(type, nameof(type));

        return type.IsAssignableTo(typeof(TTarget));
    }

    /// <summary>
    /// Determines whether an instance of this type can be assigned to
    /// an instance of the <paramref name="targetType"></paramref>.
    /// Internally uses <see cref="Type.IsAssignableFrom"/> (as reverse).
    /// </summary>
    /// <param name="type">this type</param>
    /// <param name="targetType">Target type</param>
    public static bool IsAssignableTo([NotNull] this Type type, [NotNull] Type targetType)
    {
        Check.EnsureNotNull(type, nameof(type));
        Check.EnsureNotNull(targetType, nameof(targetType));

        return targetType.IsAssignableFrom(type);
    }

    /// <summary>
    /// Gets all base classes of this type.
    /// </summary>
    /// <param name="type">The type to get its base classes.</param>
    /// <param name="includeObject">True, to include the standard <see cref="object"/> type in the returned array.</param>
    public static Type[] GetBaseClasses([NotNull] this Type type, bool includeObject = true)
    {
        Check.EnsureNotNull(type, nameof(type));

        var types = new List<Type>();
        AddTypeAndBaseTypesRecursively(types, type.BaseType, includeObject);
        return types.ToArray();
    }

    /// <summary>
    /// Gets all base classes of this type.
    /// </summary>
    /// <param name="type">The type to get its base classes.</param>
    /// <param name="stoppingType">A type to stop going to the deeper base classes. This type will be be included in the returned array</param>
    /// <param name="includeObject">True, to include the standard <see cref="object"/> type in the returned array.</param>
    public static Type[] GetBaseClasses([NotNull] this Type type, Type stoppingType, bool includeObject = true)
    {
        Check.EnsureNotNull(type, nameof(type));

        var types = new List<Type>();
        AddTypeAndBaseTypesRecursively(types, type.BaseType, includeObject, stoppingType);
        return types.ToArray();
    }

    private static void AddTypeAndBaseTypesRecursively([NotNull] ICollection<Type> types, Type type, bool includeObject, Type stoppingType = null)
    {
        if (type == null || type == stoppingType)
        {
            return;
        }

        if (!includeObject && type == typeof(object))
        {
            return;
        }

        AddTypeAndBaseTypesRecursively(types, type.BaseType, includeObject, stoppingType);
        types.Add(type);
    }

    /// <summary>
    /// Detect whether the method is async method.
    /// </summary>
    /// <param name="method"></param>
    /// <returns></returns>
    /// <exception cref="NullReferenceException"></exception>
    public static bool IsAsync([NotNull] this MethodInfo method)
    {
        if (method == null)
        {
            throw new NullReferenceException("The method instance is null.");
        }

        var returnType = method.ReturnType;
        return returnType == typeof(Task) || (returnType.IsGenericType && returnType.GetInterfaces().Any(type => type == typeof(IAsyncResult)));
    }

    /// <summary>
    /// Gets the property type.
    /// </summary>
    /// <param name="propertyType"></param>
    /// <returns></returns>
    public static Type GetPropertyType(this Type propertyType)
    {
        if (propertyType.IsGenericType && (propertyType.GetGenericTypeDefinition() == typeof(Nullable<>)))
        {
            return Nullable.GetUnderlyingType(propertyType);
        }

        return propertyType;
    }

    /// <summary>
    /// Detect whether the specified type is extends the target type.
    /// </summary>
    /// <param name="type"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static bool IsExtends<T>(this Type type)
    {
        return type.IsExtends(typeof(T));
    }

    /// <summary>
    /// Detect whether the specified type is extends the target type.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="targetType"></param>
    /// <returns></returns>
    public static bool IsExtends(this Type type, Type targetType)
    {
        var baseType = type.BaseType;

        while (baseType != typeof(object))
        {
            if (baseType == targetType)
            {
                return true;
            }

            baseType = type.BaseType;
        }

        return false;
    }
}