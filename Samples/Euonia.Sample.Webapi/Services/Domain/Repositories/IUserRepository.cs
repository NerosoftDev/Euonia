using System.Linq.Expressions;
using Nerosoft.Euonia.Sample.Domain.Aggregates;

namespace Nerosoft.Euonia.Sample.Domain.Repositories;

public interface IUserRepository
{
	Task<User> InsertAsync(User entity, bool autoSave, CancellationToken cancellationToken = default);

	Task UpdateAsync(User entity, bool autoSave, CancellationToken cancellationToken = default);

	Task<User> GetAsync(string id, bool tracking, CancellationToken cancellationToken = default);

	Task<User> GetAsync(string id, bool tracking, string[] properties, CancellationToken cancellationToken = default);

	/// <summary>
	/// Check if any user exists with the given expression.
	/// </summary>
	/// <param name="expression"></param>
	/// <param name="cancellationToken"></param>
	/// <returns></returns>
	Task<bool> AnyAsync(Expression<Func<User, bool>> expression, CancellationToken cancellationToken = default);

	Task<List<User>> FindAsync(Expression<Func<User, bool>> predicate, Func<IQueryable<User>, IQueryable<User>> handle, CancellationToken cancellationToken = default);

	Task<List<User>> FindAsync(Expression<Func<User, bool>> predicate, string[] properties, int skip, int take, CancellationToken cancellationToken = default);
}
