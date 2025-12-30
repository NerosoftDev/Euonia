namespace Nerosoft.Euonia.Uow;

public interface IUnitOfWorkContext : IDisposable
{
	Task SaveChangesAsync(CancellationToken cancellationToken = default);
	
	Task CommitAsync(CancellationToken cancellationToken = default);

	Task RollbackAsync(CancellationToken cancellationToken = default);
}