using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Nerosoft.Euonia.Repository;
using Nerosoft.Euonia.Sample.Domain.Aggregates;
using Nerosoft.Euonia.Sample.Domain.Repositories;

namespace Nerosoft.Euonia.Sample.Persist.Repositories;

internal class UserRepository(IContextProvider provider)
	: BaseRepository<SampleDataContext, User, string>(provider), IUserRepository
{
	public Task<List<User>> FindAsync(Expression<Func<User, bool>> predicate, string[] properties, int skip, int take, CancellationToken cancellationToken = default)
	{
		return FindAsync(predicate, Handle, cancellationToken);

		IQueryable<User> Handle(IQueryable<User> query)
		{
			if (properties?.Length > 0)
			{
				query = properties.Aggregate(query, (current, property) => current.Include(property));
			}

			return query.OrderByDescending(t => t.CreatedAt)
						.Skip(skip)
						.Take(take);
		}
	}
}
