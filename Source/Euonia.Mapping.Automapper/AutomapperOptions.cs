using System.Reflection;
using AutoMapper;
using Nerosoft.Euonia.Collections;

namespace Nerosoft.Euonia.Mapping;

/// <summary>
/// The automapper options.
/// </summary>
public class AutomapperOptions
{
    internal List<Action<MapperConfigurationExpression>> Configurators { get; } = new();

    internal ITypeList<Profile> ValidatingProfiles { get; } = new TypeList<Profile>();

    /// <summary>
    /// 
    /// </summary>
    /// <param name="assembly"></param>
    /// <param name="validate"></param>
    public void AddMaps(Assembly assembly, bool validate = false)
    {
        Configurators.Add(expression =>
        {
            expression.AddMaps(assembly);
        });

        if (validate)
        {
            var profileTypes = assembly
                               .DefinedTypes
                               .Where(type => typeof(Profile).IsAssignableFrom(type) && !type.IsAbstract && !type.IsGenericType);

            foreach (var profileType in profileTypes)
            {
                ValidatingProfiles.AddIfNotContains(profileType);
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="validate"></param>
    /// <typeparam name="TProfile"></typeparam>
    public void AddProfile<TProfile>(bool validate = false)
        where TProfile : Profile, new()
    {
        AddProfile(typeof(TProfile), validate);
    }

    /// <summary>
    /// Add automapper profile to the configuration.
    /// </summary>
    /// <param name="profileType">The automapper profile type.</param>
    /// <param name="validate"></param>
    // ReSharper disable once MemberCanBePrivate.Global
    public void AddProfile(Type profileType, bool validate = false)
    {
        Configurators.Add(expression =>
        {
            expression.AddProfile(profileType);
        });

        if (validate)
        {
            ValidatingProfiles.AddIfNotContains(profileType);
        }
    }

    /// <summary>
    /// Add automapper profiles to the configuration.
    /// </summary>
    /// <param name="profileTypes"></param>
    /// <param name="validate"></param>
    public void AddProfile(IEnumerable<Type> profileTypes, bool validate = false)
    {
        if (profileTypes == null || !profileTypes.Any() || profileTypes.All(t => t == null))
        {
            return;
        }

        Configurators.Add(expression =>
        {
            foreach (var profileType in profileTypes)
            {
                if (profileType == null)
                {
                    continue;
                }

                expression.AddProfile(profileType);
            }
        });

        if (validate)
        {
            foreach (var profileType in profileTypes)
            {
                if (profileType == null)
                {
                    continue;
                }

                ValidatingProfiles.AddIfNotContains(profileType);
            }
        }

        {
            // prevent code check
        }
    }
}