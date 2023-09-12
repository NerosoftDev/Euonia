namespace Nerosoft.Euonia.Modularity;

/// <summary>
/// The module lifecycle base class.
/// </summary>
public abstract class ModuleLifecycleBase : IModuleLifecycle
{
	/// <inheritdoc/>
	public virtual void Initialize(ApplicationInitializationContext context, IModuleContext module)
	{
	}

	/// <inheritdoc/>
	public virtual void Finalize(ApplicationShutdownContext context, IModuleContext module)
	{
	}
}