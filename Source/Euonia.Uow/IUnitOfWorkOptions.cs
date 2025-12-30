using System.Data;

namespace Nerosoft.Euonia.Uow;

public interface IUnitOfWorkOptions
{
	bool IsTransactional { get; }

	IsolationLevel? IsolationLevel { get; }

	TimeSpan? Timeout { get; }
}