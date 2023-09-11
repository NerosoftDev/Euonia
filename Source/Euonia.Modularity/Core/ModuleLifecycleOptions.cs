using Nerosoft.Euonia.Collections;

namespace Nerosoft.Euonia.Modularity;

/// <summary>
/// The options for module lifecycle.
/// </summary>
public class ModuleLifecycleOptions
{
	/// <summary>
	/// Gets the lifecycle.
	/// </summary>
	public ITypeList<IModuleLifecycle> Lifecycle { get; } = new TypeList<IModuleLifecycle>();
}