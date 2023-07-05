namespace Nerosoft.Euonia.Validation;

/// <summary>
/// Interface IValidator
/// </summary>
/// <typeparam name="TValue">The type of the t value.</typeparam>
public interface IObjectValidator<in TValue>
{
    /// <summary>
    /// Gets or sets the error message.
    /// </summary>
    /// <value>The error message.</value>
    string Message { get; set; }

    /// <summary>
    /// Validates the specified value.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
    bool Validate(TValue value);
}