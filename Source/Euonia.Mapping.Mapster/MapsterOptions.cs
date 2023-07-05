using System.Reflection;
using Mapster;

namespace Nerosoft.Euonia.Mapping;

public class MapsterOptions
{
    public MapsterOptions()
        : this(TypeAdapterConfig.GlobalSettings)
    {
    }

    public MapsterOptions(TypeAdapterConfig configuration)
    {
        Configuration = configuration;
    }

    public TypeAdapterConfig Configuration { get; }

    public void AddProfile<TRegister>()
        where TRegister : IRegister, new()
    {
        Configuration.Apply(new TRegister());
    }

    public void AddProfile<TRegister>(Func<TRegister> factory)
        where TRegister : IRegister
    {
        Configuration.Apply(factory());
    }

    public void AddProfile(Type registerType)
    {
        if (!typeof(IRegister).IsAssignableFrom(registerType))
        {
            throw new ArgumentException($"The register type '{registerType.FullName}' must be assignable from IRegister");
        }

        Configuration.Apply((IRegister)Activator.CreateInstance(registerType));
    }

    public void AddProfile(IRegister register)
    {
        Configuration.Apply(register);
    }

    public void AddProfiles(IEnumerable<IRegister> registers)
    {
        Configuration.Apply(registers);
    }

    public void AddProfiles(params Assembly[] assemblies)
    {
        Configuration.Scan(assemblies);
    }
}