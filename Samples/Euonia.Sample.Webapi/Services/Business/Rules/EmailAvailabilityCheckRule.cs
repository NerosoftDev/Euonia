using Nerosoft.Euonia.Business;
using Nerosoft.Euonia.Sample.Business.Actuators;
using Nerosoft.Euonia.Sample.Domain.Repositories;

namespace Nerosoft.Euonia.Sample.Business.Rules;

/// <summary>
/// Represents a rule to check the availability of an email address during user updates.
/// </summary>
internal sealed class EmailAvailabilityCheckRule(IUserRepository repository) : RuleBase
{
	/// <summary>
	/// Executes the rule to verify if the email address is available.
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

		// Skip the rule if the email address is null or whitespace.
		if (string.IsNullOrWhiteSpace(target.Email))
		{
			return;
		}

		// Check if the email property has been changed.
		var changed = target.ChangedProperties.Contains(UserGeneralBusiness.EmailProperty);
		if (!changed)
		{
			return;
		}

		// Check if the email address already exists for another user.
		var exists = await repository.AnyAsync(t => t.Email == target.Email && t.Id != target.Id, cancellationToken);
		if (exists)
		{
			// Add an error result if the email address is unavailable.
			context.AddErrorResult($"Email address '{target.Email}' is unavailable.");
		}
	}
}
