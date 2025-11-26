namespace Nerosoft.Euonia.Domain;

/// <summary>
/// Represent the object has auditing information.
/// </summary>
public interface IAuditing<TUser> : ITombstone
{
	/// <summary>
	/// Gets or sets the entry creation time.
	/// </summary>
	DateTime CreatedAt { get; set; }

	/// <summary>
	/// Gets or sets the user identifier who created the entry.
	/// </summary>
	TUser CreatedBy { get; set; }

	/// <summary>
	/// Gets or sets the entry last update time.
	/// </summary>
	DateTime UpdatedAt { get; set; }

	/// <summary>
	/// Gets or sets the user identifier who last updated the entry.
	/// </summary>
	TUser UpdatedBy { get; set; }

	/// <summary>
	/// Gets or sets the entry delete time.
	/// </summary>
	TUser DeletedBy { get; set; }
}