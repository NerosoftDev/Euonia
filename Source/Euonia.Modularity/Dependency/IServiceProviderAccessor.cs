namespace Nerosoft.Euonia.Modularity;

/// <summary>
/// The accessor of <see cref="IServiceProvider"/>.
/// </summary>
public interface IServiceProviderAccessor
{
	/// <summary>
	/// Gets the <see cref="IServiceProvider"/> instance.
	/// </summary>
    IServiceProvider ServiceProvider { get; }
}
