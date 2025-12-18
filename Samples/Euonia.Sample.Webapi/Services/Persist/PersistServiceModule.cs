using Nerosoft.Euonia.Modularity;
using Nerosoft.Euonia.Repository;

namespace Nerosoft.Euonia.Sample.Persist;

/// <summary>
/// Provides persistent-level registrations and behavior for the bounded context.
/// </summary>
/// <remarks>
/// <para>
/// This module is responsible for registering persistent implementations, persistence contexts,
/// mappings and any persistent-related services required by the features within the
/// application's modular dependency injection system.
/// </para>
/// <para>
/// The module derives from <see cref="ModuleContextBase"/> and is intended to be discovered
/// and initialized by the application's modularity/bootstrapper during startup. Concrete
/// persistent types (for example, implementations of persistent interfaces or DbContext types)
/// should be registered here so they become available to other modules and application services.
/// </para>
/// </remarks>
/// <example>
/// To ensure persistent services are available at runtime, include this module in the application's
/// module registration sequence or bootstrapper. For example:
/// <code>
/// // Pseudo-code illustrating module registration
/// var bootstrapper = new ApplicationBootstrapper();
/// bootstrapper.RegisterModule&lt;PersistentServiceModule&gt;();
/// bootstrapper.Initialize();
/// </code>
/// </example>
/// <seealso cref="ModuleContextBase"/>
public class PersistServiceModule : ModuleContextBase
{
	public override void AheadConfigureServices(ServiceConfigurationContext context)
	{
		Configure<UnitOfWorkOptions>(options =>
		{
			options.IsTransactional = false;
		});
	}

	/// <summary>
	/// Configures persistent services for the bounded context.
	/// </summary>
	/// <param name="context"></param>
	public override void ConfigureServices(ServiceConfigurationContext context)
	{
		context.Services
			   .AddContextProvider()
			   .AddUnitOfWork();

		context.Services.AddDataContextFactory<SampleDataContext>();
	}
}
