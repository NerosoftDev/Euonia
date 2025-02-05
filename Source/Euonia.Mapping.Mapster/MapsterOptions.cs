using System.Reflection;
using Mapster;
using Nerosoft.Euonia.Collections;

namespace Nerosoft.Euonia.Mapping;

/// <summary>
/// The options for Mapster
/// </summary>
public class MapsterOptions
{
	/// <summary>
	/// Gets the configuration.
	/// </summary>
	internal List<Action<TypeAdapterConfig>> Configuration { get; } = new();

	internal Dictionary<Type, IRegister> Profiles { get; } = new();

	/// <summary>
	/// Adds a profile to the configuration.
	/// </summary>
	/// <typeparam name="TRegister"></typeparam>
	public void AddProfile<TRegister>()
		where TRegister : IRegister
	{
		Profiles.TryAdd(typeof(TRegister), null);
	}

	/// <summary>
	/// Adds a profile to the configuration.
	/// </summary>
	/// <param name="factory"></param>
	/// <typeparam name="TRegister"></typeparam>
	public void AddProfile<TRegister>(Func<TRegister> factory)
		where TRegister : IRegister
	{
		Profiles.TryAdd(typeof(TRegister), factory());
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

		Profiles.TryAdd(registerType, (IRegister)Activator.CreateInstance(registerType));
	}

	/// <summary>
	/// Applies a profile to the configuration.
	/// </summary>
	/// <param name="register"></param>
	public void AddProfile(IRegister register)
	{
		Profiles.TryAdd(register.GetType(), register);
	}

	/// <summary>
	/// Applies profiles to the configuration.
	/// </summary>
	/// <param name="registers"></param>
	public void AddProfiles(IEnumerable<IRegister> registers)
	{
		foreach (var register in registers)
		{
			Profiles.TryAdd(register.GetType(), register);
		}
	}

	/// <summary>
	/// Applies profiles in specified assemblies to the configuration.
	/// </summary>
	/// <param name="assemblies"></param>
	public void AddProfiles(params Assembly[] assemblies)
	{
		Configuration.Add(config => config.Scan(assemblies));
	}
}