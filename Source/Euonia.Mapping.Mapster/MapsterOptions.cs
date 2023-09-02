using System.Reflection;
using Mapster;

namespace Nerosoft.Euonia.Mapping;

/// <summary>
/// The options for Mapster
/// </summary>
public class MapsterOptions
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MapsterOptions"/> class.
    /// </summary>
    public MapsterOptions()
        : this(TypeAdapterConfig.GlobalSettings)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MapsterOptions"/> class.
    /// </summary>
    /// <param name="configuration"></param>
    public MapsterOptions(TypeAdapterConfig configuration)
    {
        Configuration = configuration;
    }

    /// <summary>
    /// Gets the configuration.
    /// </summary>
    public TypeAdapterConfig Configuration { get; }

    /// <summary>
    /// Adds a profile to the configuration.
    /// </summary>
    /// <typeparam name="TRegister"></typeparam>
    public void AddProfile<TRegister>()
        where TRegister : IRegister, new()
    {
        Configuration.Apply(new TRegister());
    }

    /// <summary>
    /// Adds a profile to the configuration.
    /// </summary>
    /// <param name="factory"></param>
    /// <typeparam name="TRegister"></typeparam>
    public void AddProfile<TRegister>(Func<TRegister> factory)
        where TRegister : IRegister
    {
        Configuration.Apply(factory());
    }

    /// <summary>
    /// Adds a profile to the configuration.
    /// </summary>
    /// <param name="registerType"></param>
    /// <exception cref="ArgumentException"></exception>
    public void AddProfile(Type registerType)
    {
        if (!typeof(IRegister).IsAssignableFrom(registerType))
        {
            throw new ArgumentException($"The register type '{registerType!.FullName}' must be assignable from IRegister");
        }

        Configuration.Apply(((IRegister)Activator.CreateInstance(registerType))!);
    }

    /// <summary>
    /// Applies a profile to the configuration.
    /// </summary>
    /// <param name="register"></param>
    public void AddProfile(IRegister register)
    {
        Configuration.Apply(register);
    }

    /// <summary>
    /// Applies profiles to the configuration.
    /// </summary>
    /// <param name="registers"></param>
    public void AddProfiles(IEnumerable<IRegister> registers)
    {
        Configuration.Apply(registers);
    }

    /// <summary>
    /// Applies profiles in specified assemblies to the configuration.
    /// </summary>
    /// <param name="assemblies"></param>
    public void AddProfiles(params Assembly[] assemblies)
    {
        Configuration.Scan(assemblies);
    }
}