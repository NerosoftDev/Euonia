using Nerosoft.Euonia.Business;
using Nerosoft.Euonia.Sample.Business.Actuators;
using Nerosoft.Euonia.Sample.Domain.Repositories;

namespace Nerosoft.Euonia.Sample.Business.Rules;

/// <summary>
/// Represents a rule to check the availability of a username during user creation.
/// </summary>
internal sealed class UsernameAvailabilityCheckRule(IConfiguration configuration, IUserRepository repository) : RuleBase
{
	/// <summary>
	/// Executes the rule to verify if the username is available.
	/// </summary>
	/// <param name="context">The rule context containing the target object.</param>
	/// <param name="cancellationToken">The cancellation token to cancel the operation.</param>
	/// <returns>A task that represents the asynchronous operation.</returns>
	public override async Task ExecuteAsync(IRuleContext context, CancellationToken cancellationToken = default)
	{
		// Ensure the target object is of type UserGeneralBusiness.
		if (context.Target is not UserGeneralBusiness target)
		{
			return;
		}

		// Skip the rule if the operation is not an insert.
		if (!target.IsInsert)
		{
			return;
		}

		// Retrieve the list of reserved usernames from the configuration.
		var reserved = configuration.GetValue<List<string>>("ReservedUsernames");

		// Check if the username is in the reserved list.
		if (reserved?.Contains(target.Username, StringComparer.InvariantCultureIgnoreCase) == true)
		{
			// Add an error result if the username is reserved.
			context.AddErrorResult($"Username '{target.Username}' is unavailable.");
			return;
		}

		// Check if the username already exists in the repository.
		var exists = await repository.AnyAsync(t => t.Username == target.Username, cancellationToken);
		if (exists)
		{
			// Add an error result if the username already exists.
			context.AddErrorResult($"Username '{target.Username}' is unavailable.");
		}
	}
}