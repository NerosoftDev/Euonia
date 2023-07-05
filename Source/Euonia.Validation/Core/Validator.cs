namespace Nerosoft.Euonia.Validation;

/// <summary>
/// The validator.
/// </summary>
public class Validator
{
    /// <summary>
    /// Validate the specified object.
    /// </summary>
    /// <param name="item"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static void Validate<T>(T item)
        where T : class
    {
        if (item == null)
        {
            throw new ArgumentNullException(nameof(item));
        }

        if (item is IValidatableObject @object)
        {
            @object.Validate();
            if (!@object.IsValid)
            {
                var errors = @object.Errors.Select(error => new ValidationResult("Value", error));
                throw new ValidationException(string.Empty, errors);
            }
        }
        else
        {
            var validator = ValidatorFactory.Create();
            validator?.Validate(item);
        }
    }
}