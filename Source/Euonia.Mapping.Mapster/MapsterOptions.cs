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

    public void AddRegister<TRegister>()
        where TRegister : IRegister, new()
    {
        Configuration.Apply(new TRegister());
    }

    public void AddRegister<TRegister>(Func<TRegister> factory)
        where TRegister : IRegister
    {
        Configuration.Apply(factory());
    }

    public void AddRegister(Type registerType)
    {
        if (!typeof(IRegister).IsAssignableFrom(registerType))
        {
            throw new ArgumentException($"The register type '{registerType.FullName}' must be assignable from IRegister");
        }

        Configuration.Apply((IRegister)Activator.CreateInstance(registerType));
    }

    public void AddRegister(IRegister register)
    {
        Configuration.Apply(register);
    }

    public void AddRegisters(IEnumerable<IRegister> registers)
    {
        Configuration.Apply(registers);
    }

    public void AddRegisters(params Assembly[] assemblies)
    {
        Configuration.Scan(assemblies);
    }
}