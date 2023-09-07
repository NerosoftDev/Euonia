using System.Collections.ObjectModel;

namespace Nerosoft.Euonia.Validation;

/// <summary>
/// Interface for validating objects.
/// </summary>
public interface IValidatableObject
{
    /// <summary>
    /// Gets the errors.
    /// </summary>
    /// <value>The errors.</value>
    ObservableCollection<string> Errors { get; }

    /// <summary>
    /// Returns true if ... is valid.
    /// </summary>
    /// <value><c>true</c> if this instance is valid; otherwise, <c>false</c>.</value>
    bool IsValid { get; }

    /// <summary>
    /// Validates this instance.
    /// </summary>
    void Validate();
}
