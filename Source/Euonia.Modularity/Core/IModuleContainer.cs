namespace Nerosoft.Euonia.Modularity;

/// <summary>
/// The contract interface of the module container.
/// </summary>
public interface IModuleContainer
{
	/// <summary>
	/// Gets the registered modules.
	/// </summary>
    IReadOnlyList<IModuleDescriptor> Modules { get; }
}
