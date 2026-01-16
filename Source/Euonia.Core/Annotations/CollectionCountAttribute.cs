using System.Globalization;

namespace System.ComponentModel.DataAnnotations;

/// <summary>
/// Attribute to validate the number of items in a collection property.
/// Ensures a collection contains at least <see cref="MinimumCount"/> items and,
/// optionally, no more than <see cref="MaximumCount"/> items. When <see cref="AllowNull"/>
/// is true, a null value will be considered valid.
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
public class CollectionCountAttribute : ValidationAttribute
{
	/// <summary>
	/// Initializes a new instance of the <see cref="CollectionCountAttribute"/> class that
	/// enforces a minimum item count.
	/// </summary>
	/// <param name="minimumCount">The minimum number of items required in the collection.</param>
	public CollectionCountAttribute(int minimumCount)
	{
		MinimumCount = minimumCount;
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="CollectionCountAttribute"/> class that
	/// enforces a minimum and maximum item count.
	/// </summary>
	/// <param name="minimumCount">The minimum number of items required in the collection.</param>
	/// <param name="maximumCount">The maximum number of items allowed in the collection.</param>
	public CollectionCountAttribute(int minimumCount, int maximumCount)
	{
		MinimumCount = minimumCount;
		MaximumCount = maximumCount;
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="CollectionCountAttribute"/> class that
	/// enforces a minimum item count and uses an error message accessor.
	/// </summary>
	/// <param name="minimumCount">The minimum number of items required in the collection.</param>
	/// <param name="errorMessageAccessor">A function that returns the error message.</param>
	public CollectionCountAttribute(int minimumCount, Func<string> errorMessageAccessor)
		: base(errorMessageAccessor)
	{
		MinimumCount = minimumCount;
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="CollectionCountAttribute"/> class that
	/// enforces a minimum and maximum item count and uses an error message accessor.
	/// </summary>
	/// <param name="minimumCount">The minimum number of items required in the collection.</param>
	/// <param name="maximumCount">The maximum number of items allowed in the collection.</param>
	/// <param name="errorMessageAccessor">A function that returns the error message.</param>
	public CollectionCountAttribute(int minimumCount, int maximumCount, Func<string> errorMessageAccessor)
		: base(errorMessageAccessor)
	{
		MinimumCount = minimumCount;
		MaximumCount = maximumCount;
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="CollectionCountAttribute"/> class that
	/// enforces a minimum item count and uses a static error message.
	/// </summary>
	/// <param name="minimumCount">The minimum number of items required in the collection.</param>
	/// <param name="errorMessage">The error message to use when validation fails.</param>
	public CollectionCountAttribute(int minimumCount, string errorMessage)
		: base(errorMessage)
	{
		MinimumCount = minimumCount;
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="CollectionCountAttribute"/> class that
	/// enforces a minimum and maximum item count and uses a static error message.
	/// </summary>
	/// <param name="minimumCount">The minimum number of items required in the collection.</param>
	/// <param name="maximumCount">The maximum number of items allowed in the collection.</param>
	/// <param name="errorMessage">The error message to use when validation fails.</param>
	public CollectionCountAttribute(int minimumCount, int maximumCount, string errorMessage)
		: base(errorMessage)
	{
		MinimumCount = minimumCount;
		MaximumCount = maximumCount;
	}

	/// <summary>
	/// Gets or sets the minimum number of items required in the collection.
	/// Default is 0.
	/// </summary>
	public int MinimumCount { get; }

	/// <summary>
	/// Gets or sets the optional maximum number of items allowed in the collection.
	/// When null, no upper bound is enforced.
	/// </summary>
	public int? MaximumCount { get; }

	/// <summary>
	/// Gets or sets a value indicating whether a null collection is considered valid.
	/// Default is true.
	/// </summary>
	public bool AllowNull { get; set; } = true;

	/// <summary>
	/// Validates the specified value with respect to the configured minimum/maximum counts.
	/// Returns <see cref="ValidationResult.Success"/> when the value is valid; otherwise
	/// returns a <see cref="ValidationResult"/> containing a formatted error message.
	/// Behavior:
	/// - If value is null and <see cref="AllowNull"/> is true, validation succeeds.
	/// - If value is null and <see cref="AllowNull"/> is false, a validation error is returned.
	/// - If value implements <c>ICollection</c> and its <c>Count</c> is less than
	///   <see cref="MinimumCount"/>, a validation error is returned.
	/// - If value implements <c>ICollection</c> and <see cref="MaximumCount"/> has a value
	///   and <c>Count</c> is greater than <see cref="MaximumCount"/>, a validation error is returned.
	/// </summary>
	/// <param name="value">The value of the property to validate (expected to be a collection).</param>
	/// <param name="validationContext">The context information about the validation operation.</param>
	/// <returns>A <see cref="ValidationResult"/> indicating success or failure.</returns>
	protected override ValidationResult IsValid(object value, ValidationContext validationContext)
	{
		return value switch
		{
			null when AllowNull => ValidationResult.Success,
			null => new ValidationResult(ErrorMessage ?? $"The collection must not be null."),
			ICollection collection when collection.Count < MinimumCount =>
				new ValidationResult(FormatErrorMessage(ErrorMessage ?? $"The {0} must contain at least {MinimumCount} items.", validationContext.DisplayName), [validationContext.MemberName]),
			ICollection collection when MaximumCount.HasValue && collection.Count > MaximumCount.Value =>
				new ValidationResult(FormatErrorMessage(ErrorMessage ?? $"The {0} must contain at most {MaximumCount.Value} items.", validationContext.DisplayName), [validationContext.MemberName]),
			_ => ValidationResult.Success
		};
	}

	/// <summary>
	/// Formats an error message using the current culture and the provided display name.
	/// The message is expected to contain a single format placeholder ({0}) for the display name.
	/// </summary>
	/// <param name="message">The message template to format.</param>
	/// <param name="displayName">The display name of the validated member.</param>
	/// <returns>The formatted error message.</returns>
	private static string FormatErrorMessage(string message, string displayName)
	{
		return string.Format(CultureInfo.CurrentCulture, message, displayName);
	}
}