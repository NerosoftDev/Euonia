using System.Globalization;
using System.Reflection;

namespace Nerosoft.Euonia.Caching.Internal;

internal static class CacheReflectionHelper
{
    internal static CacheBackplane CreateBackplane(CacheManagerConfiguration configuration)
    {
        Check.EnsureNotNull(configuration, nameof(configuration));

        if (configuration.BackplaneType != null)
        {
            if (!configuration.CacheHandleConfigurations.Any(p => p.IsBackplaneSource))
            {
                throw new InvalidOperationException(
                    "At least one cache handle must be marked as the backplane source if a backplane is defined via configuration.");
            }

            CheckExtends<CacheBackplane>(configuration.BackplaneType);

            var args = new object[] { configuration };
            if (configuration.BackplaneTypeArguments != null)
            {
                args = configuration.BackplaneTypeArguments.Concat(args).ToArray();
            }

            return (CacheBackplane)CreateInstance(configuration.BackplaneType, args);
        }

        return null;
    }

    internal static ICollection<BaseCacheHandle<TCacheValue>> CreateCacheHandles<TCacheValue>(BaseCacheManager<TCacheValue> manager)
    {
        Check.EnsureNotNull(manager, nameof(manager));
        var managerConfiguration = manager.Configuration;
        var handles = new List<BaseCacheHandle<TCacheValue>>();

        foreach (var handleConfiguration in managerConfiguration.CacheHandleConfigurations)
        {
            var handleType = handleConfiguration.HandleType;

            Type instanceType;

            ValidateCacheHandleGenericTypeArguments(handleType);

            // if the configured type doesn't have a generic type definition ( <T> is not
            // defined )

            if (handleType.GetTypeInfo().IsGenericTypeDefinition)

            {
                instanceType = handleType.MakeGenericType(new Type[] { typeof(TCacheValue) });
            }
            else
            {
                instanceType = handleType;
            }

            var types = new List<object>(new object[] { managerConfiguration, manager, handleConfiguration });
            if (handleConfiguration.ConfigurationTypes.Length > 0)
            {
                types.AddRange(handleConfiguration.ConfigurationTypes);
            }


            if (CreateInstance(instanceType, types.ToArray()) is not BaseCacheHandle<TCacheValue> instance)
            {
                throw new InvalidOperationException("Couldn't initialize handle of type " + instanceType.FullName);
            }

            handles.Add(instance);
        }

        if (handles.Count == 0)
        {
            throw new InvalidOperationException("No cache handles defined.");
        }

        // validate backplane is the last handle in the cache manager (only if backplane is configured)
        if (handles.Any(p => p.Configuration.IsBackplaneSource) && manager.Configuration.BackplaneType != null)
        {
            if (!handles.Last().Configuration.IsBackplaneSource)
            {
                throw new InvalidOperationException("The last cache handle should be the backplane source.");
            }
        }

        return handles;
    }

    internal static object CreateInstance(Type instanceType, object[] knownInstances)
    {
        var constructors = instanceType.GetTypeInfo().DeclaredConstructors;

        constructors = constructors.Where(p => !p.IsStatic && p.IsPublic)
                                   .OrderByDescending(p => p.GetParameters().Length)
                                   .ToArray();

        if (!constructors.Any())
        {
            throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "No matching public non static constructor found for type {0}.", instanceType.FullName));
        }

        var args = MatchArguments(constructors, knownInstances);

        try
        {
            return Activator.CreateInstance(instanceType, args);
        }
        catch (Exception ex)
        {
            var exception = ex.InnerException ?? ex;

            throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Failed to initialize instance of type {0}.", instanceType), exception);
        }
    }

    private static object[] MatchArguments(IEnumerable<ConstructorInfo> constructors, object[] instances)
    {
        ParameterInfo lastParamMiss = null;
        ConstructorInfo lastCtor = null;

        foreach (var constructor in constructors)
        {
            lastCtor = constructor;
            var args = new List<object>();
            var parameters = constructor.GetParameters();
            var instancesCopy = new List<object>(instances);

            foreach (var param in parameters)
            {
                var paramValue = instancesCopy
                    .Where(p => p != null)
                    .FirstOrDefault(p => param.ParameterType.GetTypeInfo().IsAssignableFrom(p.GetType().GetTypeInfo()));

                if (paramValue == null)
                {
                    lastParamMiss = param;
                    break;
                }

                // fixing #112 by not looking at the same instance again which was already added to the args
                instancesCopy.Remove(paramValue);
                args.Add(paramValue);
            }

            if (parameters.Length == args.Count)
            {
                return args.ToArray();
            }
        }

        if (constructors.Any(p => p.GetParameters().Length == 0))
        {
			// no match found, will try empty ctor
			return Array.Empty<object>();
        }

        // give more detailed error of what failed
        if (lastCtor != null && lastParamMiss != null)
        {
            var ctorTypes = string.Join(", ", lastCtor.GetParameters().Select(p => p.ParameterType.Name).ToArray());

            throw new InvalidOperationException(
                $"Could not find a matching constructor for type '{lastCtor.DeclaringType?.Name}'. Trying to match [{ctorTypes}] but missing {lastParamMiss.ParameterType.Name}");
        }

        throw new InvalidOperationException(
            $"Could not find a matching or empty constructor for type '{lastCtor?.DeclaringType?.Name}'.");
    }

    private static IEnumerable<Type> GetGenericBaseTypes(this Type type)
    {
        var baseType = type.GetTypeInfo().BaseType;
        if (baseType == null || !baseType.GetTypeInfo().IsGenericType)
        {
            return Enumerable.Empty<Type>();
        }

        var genericBaseType = baseType.GetTypeInfo().IsGenericTypeDefinition ? baseType : baseType.GetGenericTypeDefinition();
        return Enumerable.Repeat(genericBaseType, 1)
            .Concat(baseType.GetGenericBaseTypes());
    }

    private static void ValidateCacheHandleGenericTypeArguments(Type handle)
    {
        // not really needed due to the generic type from callees being restricted.
        if (!handle.GetGenericBaseTypes().Any(p => p == typeof(BaseCacheHandle<>)))
        {
            throw new InvalidOperationException(
                string.Format(
                    CultureInfo.InvariantCulture,
                    "Configured cache handle does not implement base cache handle [{0}].",
                    handle.ToString()));
        }

        if (handle.IsGenericType && !handle.IsGenericTypeDefinition)
        {
            throw new InvalidOperationException(
                string.Format(
                    CultureInfo.InvariantCulture,
                    "Cache handle type [{0}] should not have any generic arguments defined.",
                    handle.ToString()));
        }
    }

    private static void CheckExtends<TValid>(Type type)
    {
        var isExtendsType = type.IsExtends<TValid>(); //typeof(TValid).IsAssignableFrom(type);
        if (isExtendsType)
        {
            return;
        }

        throw new InvalidOperationException($"Type {type.FullName} does not extend from {typeof(TValid).Name}.");
    }
}
