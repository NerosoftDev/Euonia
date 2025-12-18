using Nerosoft.Euonia.Domain;
using Nerosoft.Euonia.Sample.Constants;

namespace Nerosoft.Euonia.Sample.Domain.Aggregates;

/// <summary>
/// Represents a role assigned to a user.
/// </summary>
public class UserRole : Entity<string>, IHasCreateTime
{
	/// <summary>
	/// Initializes a new instance of the <see cref="UserRole"/> class.
	/// This constructor is private to enforce controlled creation of instances.
	/// </summary>
	private UserRole()
	{
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="UserRole"/> class with the specified name.
	/// </summary>
	/// <param name="name">The name of the role.</param>
	private UserRole(string name)
		: this()
	{
		Name = name;
	}

	/// <summary>
	/// Gets or sets the name of the role.
	/// </summary>
	public string Name { get; set; }

	/// <summary>
	/// Gets or sets the ID of the user associated with this role.
	/// </summary>
	public string UserId { get; set; }

	/// <summary>
	/// Gets or sets the creation time of the role.
	/// </summary>
	public DateTime CreatedAt { get; set; }

	/// <summary>
	/// Gets or sets the user associated with this role.
	/// </summary>
	public User User { get; set; }

	/// <summary>
	/// Creates a new instance of the <see cref="UserRole"/> class with the specified name.
	/// </summary>
	/// <param name="name">The name of the role.</param>
	/// <returns>A new instance of the <see cref="UserRole"/> class.</returns>
	internal static UserRole Create(string name)
	{
		name = name.Trim().ToLowerInvariant();
		if (!RoleName.All.Contains(name))
		{
			throw new ArgumentException($"Invalid role name '{name}'", nameof(name));
		}

		{
		}

		return new UserRole(name);
	}
}
