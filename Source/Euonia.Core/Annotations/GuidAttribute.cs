namespace System.ComponentModel.DataAnnotations;

/// <summary>
/// Validates that a value represents a valid GUID string.
/// Can be applied to properties, fields, or parameters.
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
public class GuidAttribute : ValidationAttribute
{
	/// <summary>
	/// Determines whether the specified value is a valid GUID.
	/// Validation rules:
	/// - null is considered valid (use [Required] to disallow nulls).
	/// - a string that can be parsed by <see cref="Guid.TryParse(string, out Guid)"/> is valid.
	/// - all other values are invalid.
	/// </summary>
	/// <param name="value">The value of the member being validated.</param>
	/// <param name="validationContext">Contextual information about the validation operation.</param>
	/// <returns>
	/// <see cref="ValidationResult.Success"/> when the value is valid; otherwise a <see cref="ValidationResult"/>
	/// containing the configured <see cref="ValidationAttribute.ErrorMessage"/> (or a default message) and the member name.
	/// </returns>
	protected override ValidationResult IsValid(object value, ValidationContext validationContext)
	{
		return value switch
		{
			null => ValidationResult.Success,
			string str when Guid.TryParse(str, out _) => ValidationResult.Success,
			_ => new ValidationResult(
				ErrorMessage ?? $"{validationContext.MemberName} must be a valid GUID.",
				[validationContext.MemberName])
		};
	}
}