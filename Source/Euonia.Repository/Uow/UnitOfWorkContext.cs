using Nerosoft.Euonia.Uow;

namespace Nerosoft.Euonia.Repository;

internal class UnitOfWorkContext : IUnitOfWorkContext
{
	public UnitOfWorkContext(IRepositoryContext context)
	{
		Context = context;
	}

	public IRepositoryContext Context { get; }

	public void Dispose()
	{
		Context.Dispose();
	}

	public Task SaveChangesAsync(CancellationToken cancellationToken = default)
	{
		return Context.SaveChangesAsync(cancellationToken);
	}

	public Task CommitAsync(CancellationToken cancellationToken = default)
	{
		return Context.SaveChangesAsync(cancellationToken);
	}

	public Task RollbackAsync(CancellationToken cancellationToken = default)
	{
		return Context.RollbackAsync(cancellationToken);
	}
}