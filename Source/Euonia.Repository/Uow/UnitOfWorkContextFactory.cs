using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nerosoft.Euonia.Uow;

namespace Nerosoft.Euonia.Repository;

/// <summary>
/// The <see cref="IContextFactory"/> implementation used to create a <see cref="IRepositoryContext"/> instance.
/// </summary>
public class UnitOfWorkContextFactory : IContextFactory
{
	private readonly IUnitOfWorkManager _manager;
	private readonly IConfiguration _configuration;

	/// <summary>
	/// 
	/// </summary>
	/// <param name="manager"></param>
	/// <param name="configuration"></param>
	public UnitOfWorkContextFactory(IUnitOfWorkManager manager, IConfiguration configuration)
	{
		_manager = manager;
		_configuration = configuration;
	}

	/// <inheritdoc />
	public TContext GetContext<TContext>()
		where TContext : class, IRepositoryContext
	{
		var unitOfWork = _manager.Current;
		if (unitOfWork == null)
		{
			return null;
		}

		if (!unitOfWork.Options.IsTransactional)
		{
			return unitOfWork.ServiceProvider.GetService<TContext>();
		}

		var contextType = typeof(TContext);

		string connectionString;

		var attribute = contextType.GetCustomAttribute<ConnectionStringAttribute>();

		if (attribute != null)
		{
			if (!string.IsNullOrWhiteSpace(attribute.Value))
			{
				connectionString = attribute.Value;
			}
			else
			{
				connectionString = _configuration.GetConnectionString(contextType.Name);
			}
		}
		else
		{
			connectionString = string.Empty;
		}

		var key = $"{contextType.FullName}_{connectionString}";

		var context = unitOfWork.FindContext(key);

		if (context is UnitOfWorkContext uowContext)
		{
			return (TContext)uowContext.Context;
		}
		else
		{
			var dbContext = unitOfWork.ServiceProvider.GetService<TContext>();
			uowContext = new UnitOfWorkContext(dbContext);
			unitOfWork.AddContext(key, uowContext);
			return dbContext;
		}

		// var context = unitOfWork.Contexts.TryGetValue(key, out var ctx)
		// 	? ctx as UnitOfWorkContext
		// 	: null;
		//
		// context ??= unitOfWork.ServiceProvider.GetService<TContext>();
		// unitOfWork.AddContext(context);
		// return context;
	}

	/// <inheritdoc />
	public int Order => 1;
}