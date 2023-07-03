using System.Text.RegularExpressions;

namespace Nerosoft.Euonia.Validation;

/// <summary>
/// Class RegularValidator.
/// Implements the <see cref="AbstractValidator{String}" />
/// </summary>
/// <seealso cref="AbstractValidator{String}" />
public class RegularValidator : AbstractValidator<string>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RegularValidator"/> class.
    /// </summary>
    /// <param name="pattern">The pattern.</param>
    /// <param name="message">The error message.</param>
    public RegularValidator(string pattern, string message)
        : base(message)
    {
        Pattern = pattern;
    }

    /// <summary>
    /// Gets the pattern.
    /// </summary>
    /// <value>The pattern.</value>
    public string Pattern { get; }

    /// <summary>
    /// Validates the specified value.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns><c>true</c> if the value is valid, <c>false</c> otherwise.</returns>
    public override bool Validate(string value)
    {
        return Regex.IsMatch(value, Pattern);
    }
}
